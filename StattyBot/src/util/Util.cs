using System;

namespace StattyBot.util {
    public class Util {
        public static string GetPositive(int i) {
            if(i > 0) return "+";
            else return "";
        }

        public static string GetPositive(long i) {
            return GetPositive((int) i);
        }

        public static string GetPositiveStr(int i) {
            return GetPositive(i) + i.ToString();
        }

        public static string GetPositiveStr(long i) {
            return GetPositiveStr((int) i);
        }
        
        public static long GetUnixTime() {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        }
    }
}