using System;

namespace LoadBadger.ConsolerRunner
{
    public class RequestTime
    {
        public DateTime Start { get; }
        public DateTime End { get; }
   
        public TimeSpan Total => End - Start;

        public RequestTime(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }
}