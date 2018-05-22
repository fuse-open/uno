using Uno.Time.Text;

namespace Uno.Time
{
    public struct Instant
    {
        private readonly long _ticks;

        public long Ticks { get { return _ticks; } }

        public Instant(long ticks)
        {
            _ticks = ticks;
        }

        public Instant PlusTicks(long ticksToAdd)
        {
            return new Instant(_ticks + ticksToAdd);
        }

        public static Instant Add(Instant left, Duration right)
        {
            return left + right;
        }

        public Instant Plus(Duration duration)
        {
            return this + duration;
        }

        internal Instant Plus(Offset offset)
        {
            return new Instant(Ticks + offset.Ticks);
        }

        public static Duration Subtract(Instant left, Instant right)
        {
            return left - right;
        }

        public Duration Minus(Instant other)
        {
            return this - other;
        }

        internal Instant Minus(Offset offset)
        {
            return new Instant(_ticks - offset.Ticks);
        }

        public static Instant Subtract(Instant left, Duration right)
        {
            return left - right;
        }

        public Instant Minus(Duration duration)
        {
            return this - duration;
        }

        // #region Operators

        public static Instant operator +(Instant left, Duration right)
        {
            return new Instant(left.Ticks + right.Ticks);
        }

        public static Duration operator -(Instant left, Instant right)
        {
            return new Duration(left.Ticks - right.Ticks);
        }

        public static Instant operator -(Instant left, Duration right)
        {
            return new Instant(left.Ticks - right.Ticks);
        }

        public static bool operator ==(Instant left, Instant right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Instant left, Instant right)
        {
            return !(left == right);
        }

        public static bool operator <(Instant left, Instant right)
        {
            return left.Ticks < right.Ticks;
        }

        public static bool operator <=(Instant left, Instant right)
        {
            return left.Ticks <= right.Ticks;
        }

        public static bool operator >(Instant left, Instant right)
        {
            return left.Ticks > right.Ticks;
        }

        public static bool operator >=(Instant left, Instant right)
        {
            return left.Ticks >= right.Ticks;
        }

        // #endregion Operators

        public static Instant FromMillisecondsSinceUnixEpoch(long milliseconds)
        {
            return FromDuration(Duration.FromMilliseconds(milliseconds));
        }

        internal static Instant FromDuration(Duration duration)
        {
            return new Instant().Plus(duration);
        }

        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            return CalendarSystem.Iso.GetInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour);
        }

        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            return CalendarSystem.Iso.GetInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute);
        }

        public override int GetHashCode()
        {
            return Ticks.GetHashCode();
        }

        public override string ToString()
        {
            return LocalDateTimePattern.GeneralIsoPattern.Format(new LocalDateTime(this));
        }

        public bool Equals(Instant other)
        {
            return Ticks == other.Ticks;
        }

        public override bool Equals(object obj)
        {
            if (obj is Instant)
            {
                return Equals((Instant)obj);
            }
            return false;
        }

        public ZonedDateTime InUtc()
        {
            return new ZonedDateTime(this, DateTimeZone.Utc, CalendarSystem.Iso);
        }

        public ZonedDateTime InZone(DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, "zone");
            return new ZonedDateTime(this, zone, CalendarSystem.Iso);
        }

        public ZonedDateTime InZone(DateTimeZone zone, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(calendar, "calendar");
            return new ZonedDateTime(this, zone, calendar);
        }

        public OffsetDateTime WithOffset(Offset offset)
        {
            return new OffsetDateTime(new LocalDateTime(this.Plus(offset)), offset);
        }

        public OffsetDateTime WithOffset(Offset offset, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, "calendar");
            return new OffsetDateTime(new LocalDateTime(this.Plus(offset), calendar), offset);
        }
    }
}
