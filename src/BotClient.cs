using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StattyBot {
    abstract class BotClient {
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

        private long LastPingTime;

        private int PingTimeout = 30;

        private byte[] ReadBuffer = new byte[7];
        private int ReadBytes;
        private bool ReadingHeader = true;

        private int UserID = -1;

        private BlockingCollection<byte[]> RequestQueue = new BlockingCollection<byte[]>();


        public BotClient(string Username, string PlainPassword, char Prefix) {
            this.Username = Username;
            this.Password = Sha256(PlainPassword);
            this.Prefix = Prefix;

            Task.Factory.StartNew(() => {
                foreach(byte[] Request in RequestQueue.GetConsumingEnumerable()) {
                    try {
                        this.stream.Write(Request, 0, Request.Length);
                    } catch {
                        Console.WriteLine("Queued Request Failed to send.");
                    }
                }
            });
            Task.Factory.StartNew(() => {
                while (true) {
                    Thread.Sleep(25000);
                    QueueRequest(new byte[]{3,0,0,0,0,0,0});
                }
            });
        }

        private void Connect() {
            try {
                client = new TcpClient("***REMOVED***", 13381);
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
                writer.WriteLine("BOT|" + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours + "|0");
                writer.Flush();

                LastPingTime = flandrecho_common.Shortcuts.GetUnixTime.Now();
            }
            catch {
                Console.WriteLine("[*.*] Connection to Flandrecho Failed!");
                FailConnection(60);
            }
        }

        private void FailConnection(int SecondsRetry) {
            LastPingTime = flandrecho_common.Shortcuts.GetUnixTime.Now();
            PingTimeout = SecondsRetry;

            Disconnect(false);
        }

        private void Disconnect(bool ResetTimeout) {
            Connected = false;
            Authenticated = false;

            if (client != null && client.Connected)
                client.Close();

        }

        public void Initialize() {
            thread = new Thread(Run);
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
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
                if(flandrecho_common.Shortcuts.GetUnixTime.Now() - LastPingTime > PingTimeout) {
                    Connect();
                    Thread.Sleep(20);
                    continue;
                }

                if (client != null && client.Connected) {

                    ushort ReadType = 0;
                    bool Compression;
                    uint Length;

                    while (Connected && stream != null && stream.DataAvailable) {

                        LastPingTime = flandrecho_common.Shortcuts.GetUnixTime.Now();

                        ReadBytes += stream.Read(ReadBuffer, ReadBytes, ReadBuffer.Length - ReadBytes);

                        if (ReadBytes == ReadBuffer.Length && ReadingHeader) {
                            ReadType = BitConverter.ToUInt16(ReadBuffer, 0);
                            Compression = ReadBuffer[2] == 1;
                            Length = BitConverter.ToUInt32(ReadBuffer, 3);

                            #if DEBUG
                            Console.WriteLine("Got packet (ID: {0} | Length: {1})", ReadType, Length.ToString());
                            #endif

                            ResetReadArray(false);
                            ReadBuffer = new byte[Length];
                        }

                        if (ReadBytes != ReadBuffer.Length) continue;

                        BinaryReader reader = new BinaryReader(new MemoryStream(ReadBuffer));

                        switch (ReadType) {
                            case 5:
                                UserID = new BinaryReader(new MemoryStream(ReadBuffer)).ReadInt32();
                                switch (UserID) {
                                    case -1:
                                        Console.WriteLine("[*.*] Authentication Failed! Invalid Login!");
                                        Thread.Sleep(2500);
                                        goto Begin;
                                    case -3:
                                        Console.WriteLine("[*.*] Your bot has been banned!");
                                        break;
                                    default:
                                        Console.WriteLine("[{0}:{1}] Bot Authenticated and running!", UserID, Username);
                                        Authenticated = true;
                                        ReadType = 0;
                                        Compression = false;
                                        Length = 0;
                                        break;
                                }
                                break;
                            case 7:
                                byte Sender_Uleb = reader.ReadByte();
                                byte Sender_Size = reader.ReadByte();
                                string Sender = Encoding.ASCII.GetString(reader.ReadBytes(Sender_Size));

                                byte Message_Uleb = reader.ReadByte();
                                byte Message_Size = reader.ReadByte();
                                string Message = Encoding.ASCII.GetString(reader.ReadBytes(Message_Size));

                                byte Target_Uleb = reader.ReadByte();
                                byte Target_Size = reader.ReadByte();
                                string Target = Encoding.ASCII.GetString(reader.ReadBytes(Target_Size));

                                if(Message[0] == Prefix)
                                    OnPrefixedMessage(Sender, Target, Message);

                                OnMessage(Sender, Target, Message);

                                break;
                            case 8:
                                #if DEBUG
                                Console.WriteLine("Received ping, sending reply");
                                #endif
                                SendPong();
                                break;
                            case 12: //Bancho_HandleOsuUpdate?
                                #if DEBUG
                                // Console.WriteLine(System.Text.Encoding.Default.GetString(ReadBuffer));
                                // Console.WriteLine(BitConverter.ToString(ReadBuffer).Replace("-","").ToLower());
                                #endif
                                break;
                            case 13: // User quit
                                break;
                            case 68: // Channel joined
                                byte unknown1 = reader.ReadByte();
                                byte ChannelLength = reader.ReadByte();
                                #if DEBUG
                                Console.WriteLine("Unknown1: " + unknown1);
                                #endif
                                string Channel = Encoding.ASCII.GetString(reader.ReadBytes(ChannelLength));
                                Console.WriteLine("Autojoining " + Channel);
                                break;
                        }
                        ResetReadArray(true);
                    }
                }
                Thread.Sleep(25);
            }
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


        public void SendMessage(string Message, string Target) {
            byte[] UlebSenderName = Uleb128_WriteString(Username);
            byte[] UlebTarget = Uleb128_WriteString(Target);
            byte[] UlebMessage = Uleb128_WriteString(Message);

            //Console.WriteLine("Sending Message: {0}", Message);

            Console.Write("IRCLog: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(String.Format("<{0}>: ", Username));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(Message + "\n");

            File.AppendAllText("irclog.txt", String.Format("IRCLog: **<{0}>**: {1}\n", Username, Message));

            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(ms)) {
                    writer.Write((short)1);
                    writer.Write((byte)0);
                    writer.Write(UlebSenderName.Length + UlebTarget.Length + UlebMessage.Length);

                    writer.Write((byte)0);
                    writer.Write(UlebMessage);
                    writer.Write(UlebTarget);
                    writer.Write(UlebSenderName);
                }

                QueueRequest(ms.ToArray());
            }
        }

        public void SendStatus(StatusList Status, bool UpdateBeatmap = true, string Text = "") {
            byte[] UlebText = Uleb128_WriteString(Text);
            byte[] UlebMapChecksum = Uleb128_WriteString("");
            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(ms)) {
                    // Packet header
                    writer.Write((short)0);
                    writer.Write((byte)0);
                    writer.Write(2 + UlebText.Length + UlebMapChecksum.Length);
                    // 2: StatusList byte & updateBeatmap bool

                    // Actual data
                    writer.Write((byte) Status); // StatusList elements are ints, cast to single byte
                    writer.Write(UpdateBeatmap); // 0x00 for false, 0x01 for true;

                    if(UpdateBeatmap) {
                        writer.Write(UlebText);
                        writer.Write(UlebMapChecksum);
                        writer.Write(0); // Current Mods
                    }
                }
                QueueRequest(ms.ToArray());
            }
        }

        private void SendPong() {
            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(ms)) {
                    writer.Write((short)4);
                    writer.Write((byte)0);
                    writer.Write(0);
                }
                QueueRequest(ms.ToArray());
            }
        }

        public void SendStatus(Status status) {
            SendStatus(status.StatusType, status.UpdateBeatmap, status.StatusText);
        }

        public virtual void OnPrefixedMessage(string Sender, string Target, string Message) { }
        public virtual void OnMessage(string Sender, string Target, string Message) {
            Console.WriteLine("IRCLog: [{0}] <{1}>: {2}", Target, Sender, Message);
            File.AppendAllText("irclog.txt",String.Format("IRCLog: [{0}] <{1}>: {2}\n", Target, Sender, Message));
        }
    }
}
