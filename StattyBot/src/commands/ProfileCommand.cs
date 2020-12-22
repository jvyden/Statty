using System;
using System.Threading.Tasks;
using StattyBot.structs;
using StattyBot.util;

namespace StattyBot.commands {
    public class ProfileCommand : Command {
        public ProfileCommand() : base("profile", new[]{""}) {}

        public override void Run(Statty client, string sender, string target, string[] Args) {
            string username;
            try {
                if(Args[1] != "") username = Args[1];
                else throw new Exception();
            } catch {
                username = sender; 
            }

            string english = sender == username ? "Your" : "Their";

            Task<User> task = new ApiHandler().UserProfile(username);
            task.ContinueWith((Task<User> task) => {
                int id = (int) task.Result.UserId;
                client.SendMessage(String.Format("{0}: {1} profile is at http://oldsu.ayyeve.xyz/user?u={2}", sender, english, id), target);
            });
        }
    }
}