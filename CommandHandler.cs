using System;

namespace Flandre_chan_tcp {
    class CommandHandler {
        private BotClient client;
        public CommandHandler(Flandre_chan client) {
            this.client = client;
        }
        public void run(string sender, string target, string cmd, string args) {
            // Console.WriteLine(cmd);
            // Console.WriteLine(args);

            switch(cmd) {
                case "roll": {
                        int max = 100;

                        string[] Split = args.Split(' ');
                        try {
                            max = int.Parse(Split[0]);
                            //Console.WriteLine("max: {0}", max);
                        }
                        catch (Exception e) {
                            //Console.WriteLine("Failed: {0}", Split[1]);
                        }

                        int RandomNumber = new Random().Next(0, max);
                        client.SendMessage(sender + " rolled a " + RandomNumber.ToString() + "!", target);
                    break;
                }
            }
        }
    }
}