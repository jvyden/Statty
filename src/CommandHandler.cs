using System;
using System.Threading.Tasks;

namespace StattyBot {
    class CommandHandler {
        private Statty client;
        private DBHandler dbHandler = new DBHandler();
        public CommandHandler(Statty client) {
            this.client = client;
        }
        public void run(string sender, string target, string cmd, string args) {
            switch(cmd) {
                case "help": {
                    client.SendMessage("Hi! I'm a Tillerino clone designed to help you track your stats easier.", target);
                    System.Threading.Thread.Sleep(100);
                    client.SendMessage("Please bear in mind this is in very early stages, and may be buggy. Please send bug reports to jvy#3348 on discord.", target);
                    System.Threading.Thread.Sleep(100);
                    client.SendMessage("Commands: $help, $roll, $update", target);
                    break;
                }
                case "roll": {
                    int max = 100;

                    string[] Split = args.Split(' ');
                    try {
                        max = int.Parse(Split[0]);
                        //Console.WriteLine("max: {0}", max);
                    }
                    catch {}

                    int RandomNumber = new Random().Next(0, max);
                    client.SendMessage(sender + " rolled a " + RandomNumber.ToString() + "!", target);
                    break;
                }
                case "id": {
                    string username;

                    string[] Split = args.Split(' ');
                    try {
                        username = Split[0];
                    }
                    catch {
                        username = sender;
                    }
                    
                    Task<User> task = new APIHandler().userProfile(username);
                    string english = sender == username ? "Your" : "The";
                    task.ContinueWith((Task<User> task) => {
                        client.SendMessage(String.Format("{0}: {1} ID is {2}.", sender, english, task.Result.UserId), target);
                    });
                    break;
                }
                case "u":
                case "update": {
                    Task<User> task = new APIHandler().userProfile(sender);
                    task.ContinueWith((Task<User> task) => {
                        int id = (int) task.Result.UserId;
                        User user = task.Result;

                        if(!dbHandler.doesUserExist(id)) {
                            client.SendMessage(sender + ": One moment, I'm adding you to my database.", target);
                            dbHandler.addUser(id);
                        }

                        InternalUser internalUser = dbHandler.getInternalUser(id);
                        long diffScore = user.RankedScore - internalUser.score;
                        long diffPlaycount = 0;
                        long diffRank = user.GlobalRank - internalUser.rank;

                        client.SendMessage(
                            String.Format("{0}: Score: {1} | Playcount: {2} | Rank: {3}", 
                                sender, Util.getPositiveStr(diffScore), Util.getPositiveStr(diffPlaycount), Util.getPositiveStr(-diffRank)
                            ), target);

                        dbHandler.updateUser(id, user.RankedScore, 0, (int) user.GlobalRank);
                    });
                    break;
                }
                case "status": {
                    int id = 1;

                    string[] Split = args.Split(' ');
                    try {
                        id = int.Parse(Split[0]);
                    }
                    catch {}

                    if(id < 0 || id > 12) {
                        client.SendMessage("Out of range (0-12)", target);
                        break;
                    }

                    client.SendStatus((StatusList) id);
                    break;
                }
            }
        }
    }
}