using Uno.Compiler.ExportTargetInterop;
using Uno.Time;

namespace Uno
{
    [DotNetType("System.DateTimeKind")]
    public enum DateTimeKind
    {
        Utc = 1,
    }

    [DotNetType("System.DateTime")]
    public struct DateTime
    {
        static Instant DotNetTimeBase { get { return Instant.FromUtc(1, 1, 1, 0, 0); } }

        static Duration DotNetTimeOffset
        {
            get {
                var UnixEpoch = Instant.FromUtc(1970, 1, 1, 0, 0);
                return UnixEpoch - DotNetTimeBase;
            }
        }

        readonly DateTimeKind _kind;
        internal readonly ZonedDateTime _time;

        public DateTime(long ticks, DateTimeKind kind)
        {
            _kind = kind;
            _time = new ZonedDateTime(new Instant(ticks) - DotNetTimeOffset, DateTimeZone.Utc);
        }

        internal DateTime(ZonedDateTime time)
        {
            _kind = DateTimeKind.Utc;
            _time = time.WithZone(DateTimeZone.Utc);
        }

        extern(DOTNET)
        internal DateTime(int year, int month, int day);

        // It's possible to be constructed by the default ctor, in which case _time will be null, so
        //  this should be used instead wherever possible in this impl
        ZonedDateTime InternalTimeOrDefault()
        {
            return _time ?? new ZonedDateTime(DotNetTimeBase, DateTimeZone.Utc);
        }

        internal Uno.Time.LocalDateTime LocalDateTime { get { return InternalTimeOrDefault().LocalDateTime; } }

        internal static DateTime Now
        {
            get
            {
                return new DateTime(ZonedDateTime.Now);
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                return new DateTime(ZonedDateTime.Now.WithZone(DateTimeZone.Utc));
            }
        }

        public DateTimeKind Kind { get { return _kind; } }

        public long Ticks { get { return (InternalTimeOrDefault().Instant + DotNetTimeOffset).Ticks; } }

        internal int Year { get { return InternalTimeOrDefault().Year; } }
        internal int Month { get { return InternalTimeOrDefault().Month; } }
        internal int Day { get { return InternalTimeOrDefault().Day; } }
        internal int Hour { get { return InternalTimeOrDefault().Hour; } }
        internal int Minute { get { return InternalTimeOrDefault().Minute; } }
        internal int Second { get { return InternalTimeOrDefault().Second; } }

        public override bool Equals(object obj)
        {
            return obj is DateTime && this == (DateTime)obj;
        }

        public override int GetHashCode()
        {
            var t = Ticks;
            return (int)(t >> 32) ^ (int)t;
        }

        public static bool operator ==(DateTime x, DateTime y)
        {
            return x.Ticks == y.Ticks;
        }

        public static bool operator !=(DateTime x, DateTime y)
        {
            return !(x == y);
        }

        public DateTime ToUniversalTime()
        {
            // Since we only suppport DateTimeKind.Utc currently, we'll just return a new object with the same value
            return new DateTime(Ticks, Kind);
        }

        public override string ToString()
        {
            switch (_kind)
            {
                case DateTimeKind.Utc:
                    return
                        Month.ToString().PadLeft(2, '0') + "/" + Day.ToString().PadLeft(2, '0') + "/" + Year.ToString().PadLeft(4, '0') +
                        " " +
                        Hour.ToString().PadLeft(2, '0') + ":" + Minute.ToString().PadLeft(2, '0') + ":" + Second.ToString().PadLeft(2, '0');

                default:
                    throw new NotImplementedException();
            }
        }
    }

}
