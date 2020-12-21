using System;
using System.Threading.Tasks;

namespace StattyBot {
    public class UpdateCommand : Command {
        private DBHandler dbHandler = new DBHandler();
        
        public UpdateCommand() : base("Update", new []{""}) {}

        public override void Run(Statty client, string sender, string target, string[] Args) {
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
        }

    }
}