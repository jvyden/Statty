using System;
using System.Collections.Generic;
using System.Linq;
using StattyBot.commands;
using StattyBot.structs;

namespace StattyBot.util {
    public class CommandHandler {
        private readonly Statty client;
        private readonly List<Command> commandList = new List<Command>{};

        public CommandHandler(Statty client) {
            this.client = client;

            commandList.Add(new HelpCommand());
            commandList.Add(new RollCommand());
            commandList.Add(new IDCommand());
            commandList.Add(new ProfileCommand());
            commandList.Add(new UpdateCommand());
            commandList.Add(new MapInfoCommand());
            
            #if DEBUG
                commandList.Add(new StatusCommand());
            #endif
        }

        public void AddCommand(Command command) {
            commandList.Add(command);
        }

        public ParsedCommand ParseCommand(string message) {
            List<string> split = new List<string>(message.Substring(1).Split(' '));
            string args = "";
            try {
                args = " " + String.Join(" ", split.GetRange(1, split.Count - 1)); // FIXME: i have no words other than this is FUCKED
            }
            catch {
                // ignored
            }

            return new ParsedCommand(split[0], args);
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
        }
    }
}