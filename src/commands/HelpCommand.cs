using StattyBot.structs;

namespace StattyBot.commands {
    public class HelpCommand : Command {
        public HelpCommand() : base("Help", new []{"h"}) {
            
        }

        public override void Run(Statty client, string sender, string target, string[] Args) {
            client.SendMessage("Hi! I'm a Tillerino clone designed to help you track your stats easier.", target);
            System.Threading.Thread.Sleep(100);
            client.SendMessage("Please bear in mind this is in very early stages, and may be buggy. Please send bug reports to jvy#3348 on discord.", target);
            System.Threading.Thread.Sleep(100);
            client.SendMessage("Commands: $help, $roll, $update, $profile", target);
        }
    }
}