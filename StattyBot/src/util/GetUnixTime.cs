using System;

namespace StattyBot.util {
    class GetUnixTime {
        public static long Now() {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        }
    }
}
