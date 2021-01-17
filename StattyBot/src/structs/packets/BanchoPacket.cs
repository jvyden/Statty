namespace StattyBot.structs.packets {
    public enum BanchoPacket {
        LOGIN_REPLY = (ushort) 5,
        HANDLE_IRC_MESSAGE = (ushort) 7,
        PING = (ushort) 8,
        HANDLE_OSU_UPDATE = (ushort) 12,
        HANDLE_OSU_QUIT = (ushort) 13,
        ANNOUNCE = (ushort) 25,
        CHANNEL_JOIN_SUCCESS = (ushort) 65,
        CHANNEL_AVAILABLE = (ushort) 66,
        CHANNEL_REVOKED = (ushort) 67,
        CHANNEL_AUTOJOIN = (ushort) 68,
        MATCH_UPDATE = (ushort) 27,
        MATCH_NEW = (ushort) 28,
        MATCH_DISBAND = (ushort) 29,
        LOBBY_JOIN = (ushort) 35,
        LOBBY_PART = (ushort) 36
    }
}