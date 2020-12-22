namespace StattyBot.structs {
    public abstract class Command {
        public string   Name {get;} 
        public string[] Aliases {get;}

        public Command(string Name, string[] Aliases) {
            this.Name = Name;
            this.Aliases = Aliases;
        }

        public abstract void Run(Statty client, string sender, string target, string[] Args);
    }
}