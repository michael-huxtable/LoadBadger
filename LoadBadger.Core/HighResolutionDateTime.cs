using System;
using System.Diagnostics;

namespace LoadBadger.Core
{
    public class HighResolutionDateTime
    {
        private static readonly DateTime _startTime = DateTime.UtcNow;
        private static readonly Stopwatch StopWatch = Stopwatch.StartNew();

        public static DateTime UtcNow => _startTime.AddTicks(StopWatch.Elapsed.Ticks);
    }
}