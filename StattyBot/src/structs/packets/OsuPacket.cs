namespace StattyBot.structs.packets {
    public enum OsuPacket {
        SEND_USER_STATUS = 0,
        SEND_IRC_MESSAGE = 1,
        EXIT = 2,
        REQUEST_STATUS_UPDATE = 3,
        PONG = 4,
        LOBBY_JOIN = 31,
        LOBBY_PART = 30
    }
}