using Uno.Time.Text;

namespace Uno.Time
{
    public struct Offset
    {
        public static readonly Offset Zero;
        public static readonly Offset MinValue;
        public static readonly Offset MaxValue;

        private readonly int _milliseconds;

        static Offset()
        {
            Zero = FromMilliseconds(0);
            MinValue = FromMilliseconds(-Constants.MillisecondsPerStandardDay + 1);
            MaxValue = FromMilliseconds(Constants.MillisecondsPerStandardDay - 1);
        }

        private Offset(int milliseconds)
        {
            Preconditions.CheckArgumentRange("milliseconds", milliseconds,
                -Constants.MillisecondsPerStandardDay + 1,
                Constants.MillisecondsPerStandardDay - 1);
            _milliseconds = milliseconds;
        }

        public int Milliseconds { get { return _milliseconds; } }

        public long Ticks { get { return Milliseconds * Constants.TicksPerMillisecond; } }

        public static Offset Max(Offset x, Offset y)
        {
            return x > y ? x : y;
        }

        public static Offset Min(Offset x, Offset y)
        {
            return x < y ? x : y;
        }

        public static Offset Negate(Offset offset)
        {
            return -offset;
        }

        public static Offset operator -(Offset offset)
        {
            return FromMilliseconds(-offset.Milliseconds);
        }

        public static Offset operator +(Offset offset)
        {
            return offset;
        }

        public static Offset operator +(Offset left, Offset right)
        {
            return FromMilliseconds(left.Milliseconds + right.Milliseconds);
        }

        public static Offset operator -(Offset minuend, Offset subtrahend)
        {
            return FromMilliseconds(minuend.Milliseconds - subtrahend.Milliseconds);
        }

        public static Offset Add(Offset left, Offset right)
        {
            return left + right;
        }

        public Offset Plus(Offset other)
        {
            return this + other;
        }

        public static Offset Subtract(Offset minuend, Offset subtrahend)
        {
            return minuend - subtrahend;
        }

        public Offset Minus(Offset other)
        {
            return this - other;
        }

        public static bool operator ==(Offset left, Offset right)
        {
            return left.Milliseconds.Equals(right.Milliseconds);
        }

        public static bool operator !=(Offset left, Offset right)
        {
            return !(left.Milliseconds == right.Milliseconds);
        }

        public static bool operator <(Offset left, Offset right)
        {
            return left.Milliseconds < right.Milliseconds;
        }

        public static bool operator <=(Offset left, Offset right)
        {
            return left.Milliseconds <= right.Milliseconds;
        }

        public static bool operator >(Offset left, Offset right)
        {
            return left.Milliseconds > right.Milliseconds;
        }

        public static bool operator >=(Offset left, Offset right)
        {
            return left.Milliseconds >= right.Milliseconds;
        }

        public override int GetHashCode()
        {
            return Milliseconds.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Offset)
            {
                return Equals((Offset)obj);
            }
            return false;
        }

        public bool Equals(Offset other)
        {
            return Milliseconds == other.Milliseconds;
        }

        public override string ToString()
        {
            return OffsetPattern.GeneralIsoPattern.Format(this);
        }

        public static Offset FromMilliseconds(int milliseconds)
        {
             return new Offset(milliseconds);
        }

        public static Offset FromTicks(long ticks)
        {
            return new Offset((int)(ticks / Constants.TicksPerMillisecond));
        }

        public static Offset FromHours(int hours)
        {
            return new Offset(hours * Constants.MillisecondsPerHour);
        }

        public static Offset FromHoursAndMinutes(int hours, int minutes)
        {
            return new Offset(hours * Constants.MillisecondsPerHour + minutes * Constants.MillisecondsPerMinute);
        }
    }
}
