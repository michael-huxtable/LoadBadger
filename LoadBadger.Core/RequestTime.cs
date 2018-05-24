using System;

namespace LoadBadger.Core
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

        public RequestTime(RequestTime other)
        {
            Start = other.Start;
            End = other.End;
        }

        protected bool Equals(RequestTime other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RequestTime) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode() * 397) ^ End.GetHashCode();
            }
        }
    }
}