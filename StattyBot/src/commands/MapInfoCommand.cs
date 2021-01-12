using System;
using BeatmapProcessor;
using StattyBot.structs;
using DbHandler = StattyBot.util.DbHandler;

namespace StattyBot.commands {
    public class MapInfoCommand : Command {
        private DbHandler dbHandler = new DbHandler();
        public MapInfoCommand() : base("MapInfo", new []{"np", "mi", "m"}) {}

        public override void Run(Statty client, string sender, string target, string[] args) {
            Player player = client.playerList.FindPlayer(sender);
            if(player == null) { // should NOT be possible
                client.SendMessage("This message should not appear. If it does, contact jvy#3348 on discord. 1", target);
                return;
            }

            if(player.Status != StatusList.PLAYING && player.Status != StatusList.PAUSED) {
                client.SendMessage($"You're not currently playing a map, so I cant give you any info. If you are, try restarting the map.", target);
                player.WaitingForMap = true;
                return;
            }
            
            player.WaitingForMap = false;
            StattyBeatmap beatmap = dbHandler.GetBeatmap(player.MapChecksum);
            if(beatmap != null) {
                client.SendMessage($"{beatmap.Name} | {beatmap.StarRating:n2}* | AR{beatmap.ApproachRate} | CS{beatmap.CircleSize} | OD{beatmap.OverallDifficulty} | HP{beatmap.HpDrainRate}", target);
            }
            else {
                client.SendMessage("I'm unable to find that map in my database, try again later perhaps?", target);
            }
        }
    }
}