using System;
using System.Collections.Generic;
using System.Text;

namespace flandrecho_common.Shortcuts {
    class GetUnixTime {
        public static long Now() {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        }
    }
}
