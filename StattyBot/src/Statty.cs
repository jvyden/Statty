using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using StattyBot.structs;
using StattyBot.util;

namespace StattyBot {
    public class Statty : BotClient {
        private string Sha256(string s) {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(s));
            foreach (byte theByte in crypto) {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        private string Username;
        private string Password;

        private char Prefix;

        private CommandHandler commandHandler;
        public ApiHandler ApiHandler;
        public DbHandler DbHandler;
        private InfluxDbHandler influxDbHandler = new InfluxDbHandler();
        private Environment environment = new Environment();

        public List<Status> Statuses = new List<Status>{};

        public void InitStatuses() {
            Statuses.Add(new Status(StatusList.LOBBY, "ing for bot rights"));
            Statuses.Add(new Status(StatusList.WATCHING, "you"));
            Statuses.Add(new Status(StatusList.PLAYING, "with Flandre and Beyley"));
            Statuses.Add(new Status(StatusList.MULTIPLAYING, "with Flandre and Beyley"));
            Statuses.Add(new Status(StatusList.TESTING, "my cheats :^)"));
            Statuses.Add(new Status(StatusList.SUBMITTING, "10 scores that are better than yours"));
            Statuses.Add(new Status(StatusList.WATCHING, "Beyley FC BARUSA of MIKOSU"));
        }

        public Statty(string Username, string PlainPassword, char Prefix) : base(Username, PlainPassword, Prefix) {
            this.Username = Username;
            this.Password = Sha256(PlainPassword);
            this.Prefix = Prefix;

            InitStatuses();

            // Handlers
            commandHandler = new CommandHandler(this);
            ApiHandler = new ApiHandler();

            // Tasks
            Task.Factory.StartNew(() => {
                while (true) {
                    if(this.Authenticated) {
                        this.SendStatus(Statuses[new Random().Next(0, Statuses.Count)]);
                        Thread.Sleep((30 * 1000) - 25);
                        // Overengineering (or over-engineering, or over-kill) is the act of designing a product to 
                        // be more robust or have more features than often necessary for its intended use,
                        // or for a process to be unnecessarily complex or inefficient. 
                    }
                    Thread.Sleep(25);
                }
            });

            Task.Factory.StartNew(async () => {
                while (true) {
                    if(Authenticated && environment.InfluxEnabled) {
                        int playerCount = playerList.GetPlayers().Count;
                        int lobbyCount = lobby.GetRooms().Count;
                        int playersInMulti = lobby.GetPlayerCount();
                        int playersInGame = playerList.GetPlayersIngame();
                        int playersAfk = playerList.GetPlayersAfk();
                        await influxDbHandler.WriteData(playerCount, lobbyCount, playersInMulti, playersInGame, playersAfk);
                    } else if(!Authenticated && environment.InfluxEnabled) {
                        await influxDbHandler.WriteOfflineStatus();
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public override void OnPrefixedMessage(string Sender, string Target, string Message) {
            ParsedCommand command = commandHandler.ParseCommand(Message);
            
            commandHandler.HandleCommand(Sender, Target, command.Command, command.Args);
        }

        public override void OnMessage(string Sender, string Target, string Message) {
            // Wow, I sure care about this here input!
        }
    }
}
