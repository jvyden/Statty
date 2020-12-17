namespace Flandre_chan_tcp {
    class Util {
        public static string getPositive(int i) {
            if(i > 0) return "+";
            else return "";
        }

        public static string getPositive(long i) {
            return getPositive((int) i);
        }

        public static string getPositiveStr(int i) {
            return getPositive(i) + i.ToString();
        }

        public static string getPositiveStr(long i) {
            return getPositiveStr((int) i);
        }
    }
}