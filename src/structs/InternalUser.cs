namespace StattyBot.structs {
    public class InternalUser {
        public long Id;
        public long Score;
        public long Playcount;
        public int Rank;

        public InternalUser(long id, long score, long playcount, int rank) {
            this.Id = id;
            this.Score = score;
            this.Playcount = playcount;
            this.Rank = rank;
        }
    }
}