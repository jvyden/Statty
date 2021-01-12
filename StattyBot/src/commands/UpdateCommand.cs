using System;
using System.Threading.Tasks;
using StattyBot.structs;
using StattyBot.util;

namespace StattyBot.commands {
    public class UpdateCommand : Command {
        private DbHandler dbHandler = new DbHandler();
        
        public UpdateCommand() : base("Update", new []{""}) {}

        public override void Run(Statty client, string sender, string target, string[] args) {
            Task<User> task = new ApiHandler().UserProfile(sender);
            task.ContinueWith((Task<User> task) => {
                int id = (int) task.Result.UserId;
                User user = task.Result;

                if(!dbHandler.DoesUserExist(id)) {
                    client.SendMessage(sender + ": One moment, I'm adding you to my database.", target);
                    dbHandler.AddUser(id);
                }

                InternalUser internalUser = dbHandler.GetInternalUser(id);
                long diffScore = user.RankedScore - internalUser.Score;
                long diffPlaycount = 0;
                long diffRank = user.GlobalRank - internalUser.Rank;

                client.SendMessage(
                    $"{sender}: Score: {Util.GetPositiveStr(diffScore)} | Playcount: {Util.GetPositiveStr(diffPlaycount)} | Rank: {Util.GetPositiveStr(-diffRank)}", target);

                dbHandler.UpdateUser(id, user.RankedScore, 0, (int) user.GlobalRank);
            });
        }

    }
}