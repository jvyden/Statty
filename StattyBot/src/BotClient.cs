using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StattyBot.structs;
using StattyBot.util;

namespace StattyBot {
    public abstract class BotClient {
        private string Sha256(string s) {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(s));
            foreach (byte theByte in crypto) {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        private StreamWriter writer;
        private Stream readStream;

        private TcpClient client;
        private NetworkStream stream;

        private Thread thread;

        private string Username;
        private string Password;

        private char Prefix;

        public bool Connected;
        public bool Authenticated;
        private bool Disconnecting;

        private long LastPingTime;

        private int PingTimeout = 30;

        private byte[] ReadBuffer = new byte[7];
        private int ReadBytes;
        private bool ReadingHeader = true;

        private int UserId = -1;

        public PlayerList playerList = new PlayerList();
        public Lobby lobby = new Lobby();

        private BlockingCollection<byte[]> RequestQueue = new BlockingCollection<byte[]>();

        private static Environment environment = new Environment();
        
        public BotClient(string Username, string PlainPassword, char Prefix) {
            this.Username = Username;
            this.Password = Sha256(PlainPassword);
            this.Prefix = Prefix;

            Task.Factory.StartNew(() => {
                foreach(byte[] request in RequestQueue.GetConsumingEnumerable()) {
                    try {
                        this.stream.Write(request, 0, request.Length);
                    } catch {
                        Console.WriteLine("Queued Request Failed to send.");
                    }
                }
            });
            // Task.Factory.StartNew(() => {
            //     while (true) {
            //         Thread.Sleep(25000);
            //         RequestPresence();
            //     }
            // });
        }

        private void Connect() {
            lobby.RemoveAllRooms();
            playerList.RemoveAllPlayers();
            try {
                client = new TcpClient(environment.Host, environment.Port);
                client.NoDelay = true;

                stream = client.GetStream();
                writer = new StreamWriter(stream);

                Console.WriteLine("[*.*] Authenticating...");

                Connected = true;

                writer.AutoFlush = true;

                writer.WriteLine(this.Username);
                writer.Flush();
                writer.WriteLine(this.Password);
                writer.Flush();
                #if FLAN2
                    writer.WriteLine("1400|" + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours + "|0");
                #else
                    writer.WriteLine("BOT|" + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours + "|0");
                #endif
                writer.Flush();

                LastPingTime = GetUnixTime.Now();
            }
            catch {
                Console.WriteLine("[*.*] Connection to Flandrecho Failed!");
                FailConnection(60);
            }
        }

        private void FailConnection(int SecondsRetry) {
            LastPingTime = GetUnixTime.Now();
            PingTimeout = SecondsRetry;

            Disconnect();
        }

        private void Disconnect() {
            lobby.RemoveAllRooms();
            playerList.RemoveAllPlayers();
            if(Authenticated) SendExit();
            
            Connected = false;
            Authenticated = false;

            if (client != null && client.Connected)
                client.Close();
        }

        public void Initialize() {
            thread = new Thread(Run);
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
            
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit; 
        }

        private void OnProcessExit(object sender, EventArgs e) {
            Console.WriteLine("Exiting...");
            Disconnecting = true;
        }

        private void ResetReadArray(bool Header) {
            if (Header)
                ReadBuffer = new byte[7];
            ReadingHeader = Header;
            ReadBytes = 0;
        }

        private void QueueRequest(byte[] buffer) {
            RequestQueue.Add(buffer);
        }

        public void Run() {
            Begin:
            Connect();

            while (true) {
                if(GetUnixTime.Now() - LastPingTime > PingTimeout) {
                    Connect();
                    Thread.Sleep(20);
                    continue;
                }

                if(Disconnecting) {
                    Disconnect();
                    break;
                }

                if (client != null && client.Connected) {
                    ushort readType = 0;
                    bool compression;
                    uint length;

                    while (Connected && stream != null && stream.DataAvailable) {

                        LastPingTime = GetUnixTime.Now();

                        ReadBytes += stream.Read(ReadBuffer, ReadBytes, ReadBuffer.Length - ReadBytes);

                        if (ReadBytes == ReadBuffer.Length && ReadingHeader) {
                            readType = BitConverter.ToUInt16(ReadBuffer, 0);
                            compression = ReadBuffer[2] == 1;
                            length = BitConverter.ToUInt32(ReadBuffer, 3);

                            ResetReadArray(false);
                            ReadBuffer = new byte[length];
                        }

                        if (ReadBytes != ReadBuffer.Length) continue;

                        BinaryReader reader = new BinaryReader(new MemoryStream(ReadBuffer));

                        switch (readType) {
                            case 5:
                                UserId = reader.ReadInt32();
                                switch (UserId) {
                                    case -1:
                                        Console.WriteLine("[*.*] Authentication Failed! Invalid Login!");
                                        Thread.Sleep(2500);
                                        goto Begin;
                                    case -2:
                                        Console.WriteLine("[*.*] Client too old.");
                                        break;
                                    case -3:
                                        Console.WriteLine("[*.*] Your bot has been banned!");
                                        break;
                                    default:
                                        Console.WriteLine("[{0}:{1}] Bot Authenticated and running!", UserId, Username);
                                        Authenticated = true;
                                        readType = 0;
                                        compression = false;
                                        length = 0;
                                        RequestPresence();
                                        JoinLobby();
                                        break;
                                }
                                break;
                            case 7:
                                string sender = ReadString(reader);
                                string message = ReadString(reader);
                                string target = ReadString(reader);
                                
                                Console.Write("IRCLog: ");
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.Write($"<{Username}>: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write(message + "\n");

                                if(message[0] == Prefix)
                                    OnPrefixedMessage(sender, target, message);

                                OnMessage(sender, target, message);

                                break;
                            case 8:
                                SendPong();
                                break;
                            case 12: // Bancho_HandleOsuUpdate
                                Player player = new Player();
                                
                                player.UserID = reader.ReadInt32();
                                byte clientType = reader.ReadByte(); // 0: Self, 2: Online user
                                
                                player.Status = (StatusList) reader.ReadByte();
                                bool updateBeatmap = reader.ReadBoolean();

                                if(updateBeatmap) {
                                    player.StatusText = ReadString(reader);
                                    player.MapChecksum = ReadString(reader);
                                    player.EnabledMods = reader.ReadUInt16();
                                }

                                if(clientType > 0) {
                                    player.RankedScore = reader.ReadInt64();
                                    player.Accuracy = reader.ReadSingle();
                                    player.PlayCount = reader.ReadInt32();
                                    player.TotalScore = reader.ReadInt64();
                                    player.Rank = reader.ReadUInt16();
                                }
                                if(clientType == 2) {
                                    player.Username = ReadString(reader);
                                    player.Location = ReadString(reader);
                                    player.TimeZone = reader.ReadByte();
                                    player.Country = ReadString(reader);

                                    Player oldPlayer = playerList.FindPlayer(player.Username);
                                    if(!updateBeatmap && oldPlayer != null) {
                                        player.StatusText = oldPlayer.StatusText;
                                        player.MapChecksum = oldPlayer.MapChecksum;
                                        player.EnabledMods = oldPlayer.EnabledMods;
                                    }
                                }

                                playerList.UpdatePlayer(player);
                                break;
                            case 13: // User quit
                                int userId = reader.ReadInt32();
                                playerList.RemovePlayer(userId);
                                break;
                            case 27: // Room updated
                            case 28: // Room created
                                MultiplayerRoom room = new MultiplayerRoom();

                                room.MatchId = reader.ReadByte();
                                room.InProgress = reader.ReadBoolean();
                                room.MatchType = reader.ReadByte();
                                room.ActiveMods = reader.ReadInt16();
                                room.GameName = ReadString(reader);
                                room.BeatmapName = ReadString(reader);
                                room.BeatmapId = reader.ReadInt32();
                                room.BeatmapChecksum = ReadString(reader);

                                for (int i = 0; i < 8; i++) room.UserSlotStatus[i] = (SlotStatus)reader.ReadByte();
                                for (int i = 0; i < 8; i++) {
                                    room.SlotId[i] = (room.UserSlotStatus[i] & SlotStatus.CompHasPlayer) > 0 ? reader.ReadInt32() : -1;
                                }
                                
                                lobby.UpdateRoom(room);
                                break;
                            case 29: // Room disbanded
                                int matchId = reader.ReadInt32();
                                lobby.RemoveRoom(matchId);
                                break;
                            case 68: // Channel joined
                                string channel = ReadString(reader);
                                Console.WriteLine("Auto-joining " + channel);
                                break;
                        }
                        ResetReadArray(true);
                    }
                }
                Thread.Sleep(25);
            }
        }

        public string ReadString(BinaryReader reader) {
            if(reader.ReadByte() == 0) return "";
            return reader.ReadString();
        }

        private byte[] Write_Uleb128(int num) {
            List<byte> ret = new List<byte>();

            if (num == 0) {
                return new byte[] { 0x00 };
            }

            int length = 0;

            while (num > 0) {
                ret.Add((byte)(num & 127));
                num >>= 7;
                if (num != 0) {
                    ret[length] |= 128;
                }
                length += 1;
            }

            return ret.ToArray();
        }

        private byte[] Uleb128_WriteString(string s) {

            if (s.Length == 0) {
                return new byte[] { 0x00 };
            }

            List<byte> ret = new List<byte>();

            ret.Add(11);

            ret.AddRange(Write_Uleb128(s.Length));
            ret.AddRange(Encoding.UTF8.GetBytes(s));

            return ret.ToArray();
        }
        
        // Packets

        public void SendPacketNoData(short id) {
            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(ms)) {
                    writer.Write((short)3);
                    writer.Write((byte)0);
                    writer.Write(0);
                }
                QueueRequest(ms.ToArray());
            }
        }
        
        public void SendMessage(string Message, string Target) {
            byte[] ulebSenderName = Uleb128_WriteString(Username);
            byte[] ulebTarget = Uleb128_WriteString(Target);
            byte[] ulebMessage = Uleb128_WriteString(Message);

            //Console.WriteLine("Sending Message: {0}", Message);

            Console.Write("IRCLog: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"<{Username}>: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Message + "\n");

            File.AppendAllText("irclog.txt", $"IRCLog: **<{Username}>**: {Message}\n");

            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(ms)) {
                    writer.Write((short)1);
                    writer.Write((byte)0);
                    writer.Write(ulebSenderName.Length + ulebTarget.Length + ulebMessage.Length);

                    writer.Write((byte)0);
                    writer.Write(ulebMessage);
                    writer.Write(ulebTarget);
                    writer.Write(ulebSenderName);
                }

                QueueRequest(ms.ToArray());
            }
        }

        public void SendStatus(StatusList Status, bool UpdateBeatmap = true, string Text = "") {
            byte[] ulebText = Uleb128_WriteString(Text);
            byte[] ulebMapChecksum = Uleb128_WriteString("");
            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(ms)) {
                    // Packet header
                    writer.Write((short)0);
                    writer.Write((byte)0);
                    writer.Write(2 + ulebText.Length + ulebMapChecksum.Length);
                    // 2: StatusList byte & updateBeatmap bool

                    // Actual data
                    writer.Write((byte) Status); // StatusList elements are ints, cast to single byte
                    writer.Write(UpdateBeatmap); // 0x00 for false, 0x01 for true;

                    if(UpdateBeatmap) {
                        writer.Write(ulebText);
                        writer.Write(ulebMapChecksum);
                        writer.Write(0); // Current Mods
                    }
                }
                QueueRequest(ms.ToArray());
            }
        }

        private void SendPong() {
            SendPacketNoData(4);
        }

        public void SendStatus(Status status) {
            SendStatus(status.StatusType, status.UpdateBeatmap, status.StatusText);
        }

        public void SendExit() {
            SendPacketNoData(2);
        }

        public void JoinLobby() {
            SendPacketNoData(31);
        }
        
        public void RequestPresence() {
            SendPacketNoData(3);
        }



        public virtual void OnPrefixedMessage(string Sender, string Target, string Message) { }
        public virtual void OnMessage(string Sender, string Target, string Message) { }
    }
}
