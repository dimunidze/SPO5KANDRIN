using System.Diagnostics;
namespace Lab5 {
    public static class StopWatchExtentions {
        public static long GetNanoSeconds(this Stopwatch sw) {
            return sw.ElapsedTicks * 1000000000 / Stopwatch.Frequency;
        }
    }
}
