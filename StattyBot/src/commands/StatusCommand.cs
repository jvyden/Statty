using StattyBot.structs;

namespace StattyBot.commands {
    public class StatusCommand : Command {
        public StatusCommand() : base("Status", new[]{""}) {}

        public override void Run(Statty client, string sender, string target, string[] args) {
            int id = 1;
            try {
                id = int.Parse(args[0]);
            }
            catch {}

            if(id < 0 || id > 12) {
                client.SendMessage("Out of range (0-12)", target);
                return;
            }

            client.SendStatus((StatusList) id);
        }
    }
}