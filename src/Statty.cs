using System;
using System.Collections.Generic;
using System.Text;

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

        public Statty(string Username, string PlainPassword, char Prefix) : base(Username, PlainPassword, Prefix) {
            this.Username = Username;
            this.Password = Sha256(PlainPassword);
            this.Prefix = Prefix;

            commandHandler = new CommandHandler(this);
            apiHandler = new APIHandler();
        }

        public override void OnPrefixedMessage(string Sender, string Target, string Message) {
            commandHandler.run(Sender, Target, Message.Substring(1).Split(' ')[0], Message.Substring(Message.IndexOf(' ') + 1));
        }

        public override void OnMessage(string Sender, string Target, string Message) {
            // Wow, I sure care about this here input!
        }
    }
}
