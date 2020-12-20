using System;

namespace flandrecho_common.Shortcuts {
    class GetUnixTime {
        public static long Now() {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        }
    }
}
