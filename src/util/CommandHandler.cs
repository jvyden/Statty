using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace StattyBot {
    class CommandHandler {
        private Statty client;
        private List<Command> commandList = new List<Command>{};

        public CommandHandler(Statty client) {
            this.client = client;

            commandList.Add(new HelpCommand());
            commandList.Add(new RollCommand());
            commandList.Add(new IDCommand());
            commandList.Add(new ProfileCommand());
            commandList.Add(new UpdateCommand());
            
            #if DEBUG
                commandList.Add(new StatusCommand());
            #endif
        }

        public Command FindCommandByInput(string input) {
            foreach (Command command in commandList) {
                if(String.Equals(command.Name, input, StringComparison.CurrentCultureIgnoreCase)) return command;
                if(command.Aliases.Contains(input.ToLower())) return command;
            }
            return null;
        }

        public void HandleCommand(string sender, string target, string cmd, string args) {
            string[] Split = args.Split(' ');
            Command command = FindCommandByInput(cmd);
            command?.Run(client, sender, target, Split);
            
            switch(cmd) {
                case "p":
                case "profile": {

                    break;
                }
            }
        }
    }
}