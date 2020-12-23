namespace StattyBot.structs {
    public class ParsedCommand {
        public string Command { get; }
        public string Args { get; }

        public ParsedCommand(string command, string args) {
            Command = command;
            Args = args;
        }
    }
}