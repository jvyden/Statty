using System;
using System.Collections.Generic;
using System.Linq;
using StattyBot.commands;
using StattyBot.structs;

namespace StattyBot.util {
    class CommandHandler {
        private readonly Statty client;
        private readonly List<Command> commandList = new List<Command>{};

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
                if(string.Equals(command.Name, input, StringComparison.CurrentCultureIgnoreCase)) return command;
                if(command.Aliases.Contains(input.ToLower())) return command;
            }
            return null;
        }

        public void HandleCommand(string sender, string target, string cmd, string args) {
            string[] split = args.Split(' ');
            Command command = FindCommandByInput(cmd);
            command?.Run(client, sender, target, split);
            
            switch(cmd) {
                case "p":
                case "profile": {

                    break;
                }
            }
        }
    }
}