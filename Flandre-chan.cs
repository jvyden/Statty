using System;
using System.Collections.Generic;
using System.Text;

namespace Flandre_chan_tcp {
    class Flandre_chan : BotClient {
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

        public Flandre_chan(string Username, string PlainPassword, char Prefix) : base(Username, PlainPassword, Prefix) {
            this.Username = Username;
            this.Password = Sha256(PlainPassword);
            this.Prefix = Prefix;
        }

        public override void OnPrefixedMessage(string Sender, string Target, string Message) {
            if(Message.Substring(0,5) == "!roll") {
                int max = 100;

                string[] Split = Message.Split(' ');
                try {
                    max = int.Parse(Split[1]);
                    //Console.WriteLine("max: {0}", max);
                }catch (Exception e){
                    //Console.WriteLine("Failed: {0}", Split[1]);
                }
                
                int RandomNumber = new Random().Next(0, max);
                SendMessage(Sender + " rolled a " + RandomNumber.ToString() + "!", Target);
            }
        }

        public override void OnMessage(string Sender, string Target, string Message) {
            // Wow, I sure care about this here input!
        }
    }
}
