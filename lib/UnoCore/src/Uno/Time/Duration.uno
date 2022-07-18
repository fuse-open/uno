namespace Uno.Time
{
    public struct Duration
    {
        public static readonly Duration Zero;

        public static readonly Duration Epsilon;

        internal static readonly Duration OneStandardWeek;

        internal static readonly Duration OneStandardDay;

        private static readonly Duration OneHour;

        private static readonly Duration OneMinute;

        private static readonly Duration OneSecond;

        private static readonly Duration OneMillisecond;

        static Duration()
        {
            Zero = new Duration(0L);
            Epsilon = new Duration(1L);
            OneStandardWeek = new Duration(Constants.TicksPerStandardWeek);
            OneStandardDay = new Duration(Constants.TicksPerStandardDay);
            OneHour = new Duration(Constants.TicksPerHour);
            OneMinute = new Duration(Constants.TicksPerMinute);
            OneSecond = new Duration(Constants.TicksPerSecond);
            OneMillisecond = new Duration(Constants.TicksPerMillisecond);
        }

        internal Duration(long ticks)
        {
            this.ticks = ticks;
        }

        private readonly long ticks;

        public long Ticks { get { return ticks; } }

        public static Duration operator +(Duration left, Duration right)
        {
            return new Duration(left.Ticks + right.Ticks);
        }

        public static Duration Add(Duration left, Duration right)
        {
            return left + right;
        }

        public Duration Plus(Duration other)
        {
            return this + other;
        }

        public static Duration operator -(Duration left, Duration right)
        {
            return new Duration(left.Ticks - right.Ticks);
        }

        public static Duration Subtract(Duration left, Duration right)
        {
            return left - right;
        }

        public Duration Minus(Duration other)
        {
            return this - other;
        }

        public static Duration operator /(Duration left, long right)
        {
            return new Duration(left.Ticks / right);
        }

        public static Duration Divide(Duration left, long right)
        {
            return left / right;
        }

        public static Duration operator *(Duration left, long right)
        {
            return new Duration(left.Ticks * right);
        }

        public static Duration operator *(long left, Duration right)
        {
            return new Duration(left * right.Ticks);
        }

        public static Duration Multiply(Duration left, long right)
        {
            return left * right;
        }

        public static Duration Multiply(long left, Duration right)
        {
            return left * right;
        }

        public static bool operator ==(Duration left, Duration right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Duration left, Duration right)
        {
            return !(left == right);
        }

        public static bool operator <(Duration left, Duration right)
        {
            return left.Ticks < right.Ticks;
        }

        public static bool operator <=(Duration left, Duration right)
        {
            return left.Ticks <= right.Ticks;
        }

        public static bool operator >(Duration left, Duration right)
        {
            return left.Ticks > right.Ticks;
        }

        public static bool operator >=(Duration left, Duration right)
        {
            return left.Ticks >= right.Ticks;
        }

        public static Duration operator -(Duration duration)
        {
            return new Duration(-duration.Ticks);
        }

        public static Duration Negate(Duration duration)
        {
            return -duration;
        }

        public override int GetHashCode()
        {
            return Ticks.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Duration)
            {
                return Equals((Duration)obj);
            }
            return false;
        }

        public bool Equals(Duration other)
        {
            return Ticks == other.Ticks;
        }

        public static Duration FromStandardWeeks(long weeks)
        {
            return OneStandardWeek * weeks;
        }

        public static Duration FromStandardDays(long days)
        {
            return OneStandardDay * days;
        }

        public static Duration FromHours(long hours)
        {
            return OneHour * hours;
        }

        public static Duration FromMinutes(long minutes)
        {
            return OneMinute * minutes;
        }

        public static Duration FromSeconds(long seconds)
        {
            return OneSecond * seconds;
        }

        public static Duration FromMilliseconds(long milliseconds)
        {
            return OneMillisecond * milliseconds;
        }

        public static Duration FromTicks(long ticks)
        {
            return new Duration(ticks);
        }
    }
}
