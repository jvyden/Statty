namespace StattyBot.structs {
    public class MultiplayerRoom {
        public short MatchId;
        public bool InProgress;
        public short MatchType;
        public short ActiveMods;
        public string GameName;
        public string BeatmapName;
        public int BeatmapId;
        public string BeatmapChecksum;
        
        public short PlayerCount;

        public int[] SlotId = {0, 0, 0, 0, 0, 0, 0, 0};
        public SlotStatus[] UserSlotStatus = {0, 0, 0, 0, 0, 0, 0, 0};

        public int GetPlayerCount() {
            int count = 0;
            for (int i = 0; i < UserSlotStatus.Length; i++) {
                if(UserSlotStatus[i] != SlotStatus.Locked && UserSlotStatus[i] != SlotStatus.Open) count++;
            }
            return count;
        }
    }
}