namespace StattyBot {
    public class Status {
        public StatusList StatusType;
        public bool UpdateBeatmap;
        public string StatusText;

        public Status(StatusList StatusType, string StatusText, bool UpdateBeatmap = true) {
            this.StatusType = StatusType;
            this.StatusText = StatusText;
            this.UpdateBeatmap = UpdateBeatmap;
        }
    }
}