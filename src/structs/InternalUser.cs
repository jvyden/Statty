namespace StattyBot {
    class InternalUser {
        public long id;
        public long score;
        public long playcount;
        public int rank;

        public InternalUser(long id, long score, long playcount, int rank) {
            this.id = id;
            this.score = score;
            this.playcount = playcount;
            this.rank = rank;
        }
    }
}