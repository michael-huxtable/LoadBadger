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
            return obj.GetType() == GetType() && Equals((RequestTime) obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Start);
            hashCode.Add(End);
            return hashCode.ToHashCode();
        }
    }
}