using System;

namespace StattyBot.util {
    class AccuracyCalc {
        private static int GetHits(int one, int two, int three, int four) {
            return one + two + three + four;
        }
        public static double GetAccuracy(int count50, int count100, int count300, int countMiss) {
            return Math.Round((double)(count50 * 50 + count100 * 100 + count300 * 300) / (double)(GetHits(count50, count100, count300, countMiss) * 300),2);
        }
        public static double GetAccuracy100(int count50, int count100, int count300, int countMiss) {
            return Math.Round(((double)(count50 * 50 + count100 * 100 + count300 * 300) / (double)(GetHits(count50, count100, count300, countMiss) * 300)) * 100.0, 2);
        }

    }
}
