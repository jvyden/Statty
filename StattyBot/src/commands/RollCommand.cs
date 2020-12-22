using System;
using StattyBot.structs;

namespace StattyBot.commands
{
    public class RollCommand : Command {
        public RollCommand() : base("Roll", new []{"r"}) {}

        public override void Run(Statty client, string sender, string target, string[] Args) {
            int max = 100;

            try {
                if(Args[1] != "") max = int.Parse(Args[1]);
                else throw new Exception();
            }
            catch {
                // ignored
            }

            int randomNumber = new Random().Next(0, max);
            client.SendMessage(sender + " rolled a " + randomNumber + "!", target);
        }
    }
}