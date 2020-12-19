﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace StattyBot {
    class Statty : BotClient {
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
        public APIHandler apiHandler;
        public DBHandler dbHandler;

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
            apiHandler = new APIHandler();

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
        }

        public override void OnPrefixedMessage(string Sender, string Target, string Message) {
            commandHandler.run(Sender, Target, Message.Substring(1).Split(' ')[0], Message.Substring(Message.IndexOf(' ') + 1));
        }

        public override void OnMessage(string Sender, string Target, string Message) {
            // Wow, I sure care about this here input!
        }
    }
}
