using System;
using StattyBot;
using StattyBot.structs;

namespace StattyTests {
    public class FakeCommand : Command {
        private readonly Action _callback;
        public FakeCommand(Action callback) : base("Fake", new[]{""}) {
            _callback = callback;
        }
        public override void Run(Statty client, string sender, string target, string[] args) {
            _callback();
        }
    }
}