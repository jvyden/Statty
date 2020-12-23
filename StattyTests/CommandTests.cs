using NUnit.Framework;
using StattyBot.structs;
using StattyBot.util;

namespace StattyTests {
    [TestFixture]
    public class CommandTests {
        private readonly CommandHandler _commandHandler = new CommandHandler(null);
        [Test]
        public void ParsesCommand() {
            ParsedCommand command1 = _commandHandler.ParseCommand("$help 1 2 3");

            Assert.NotNull(command1);
            Assert.True(command1.Command == "help");
            Assert.True(command1.Args == " 1 2 3");

            ParsedCommand command2 = _commandHandler.ParseCommand("$help");

            Assert.NotNull(command2);
            Assert.True(command2.Command == "help");
            Assert.True(command2.Args == " ");
        }

        [Test]
        public void FindsCommandByName() {
            Command command1 = _commandHandler.FindCommandByInput("help");
            Assert.NotNull(command1);
            Assert.True(command1.Name == "Help");
        }

        [Test]
        public void FindsCommandByAlias() {
            Command command1 = _commandHandler.FindCommandByInput("h");
            Assert.NotNull(command1);
            Assert.True(command1.Name == "Help");
        }

        [Test]
        public void RunsCommand() {
            CommandHandler commandHandler = new CommandHandler(null);
            commandHandler.AddCommand(new FakeCommand(Assert.Pass));
            
            commandHandler.HandleCommand("", "", "fake", "");
        }   
    }
}
