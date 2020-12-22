using System;
using System.Threading.Tasks;
using StattyBot.structs;
using StattyBot.util;

namespace StattyBot.commands {
    public class IDCommand : Command {
        public IDCommand() : base("ID", new[]{""}) {}

        public override void Run(Statty client, string sender, string target, string[] Args) {
            string username;
            try {
                if(Args[1] != "") username = Args[1];
                else throw new Exception();
            }
            catch {
                username = sender;
            }
                    
            Task<User> task = new ApiHandler().UserProfile(username);
            string english = sender == username ? "Your" : "The";
            task.ContinueWith((Task<User> task) => {
                client.SendMessage(String.Format("{0}: {1} ID is {2}.", sender, english, task.Result.UserId), target);
            });
        }
    }
}