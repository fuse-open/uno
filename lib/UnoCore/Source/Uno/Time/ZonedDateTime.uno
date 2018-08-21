using Uno.Time.Calendars;
using Uno.Diagnostics;

namespace Uno.Time
{
    public class ZonedDateTime
    {
        private readonly LocalDateTime _localDateTime;
        private readonly DateTimeZone _zone;
        private readonly Offset _offset;

        internal ZonedDateTime(LocalDateTime localDateTime, Offset offset, DateTimeZone zone)
        {
            _localDateTime = localDateTime;
            _offset = offset;
            _zone = zone;
        }

        public ZonedDateTime(Instant instant, DateTimeZone zone) : this(instant, zone, CalendarSystem.Iso)
        {
        }

        public ZonedDateTime(Instant instant, DateTimeZone zone, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(calendar, "calendar");
            _offset = zone.GetUtcOffset(new LocalDateTime(instant, calendar));
            _localDateTime = new LocalDateTime(instant.Plus(_offset), calendar);
            _zone = zone;
        }

        public ZonedDateTime(LocalDateTime localDateTime, DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, "zone");
            _offset = zone.GetUtcOffset(localDateTime);
            _localDateTime = localDateTime;
            _zone = zone;
        }

        public static ZonedDateTime Now
        {
            get
            {
                var ticks = Clock.GetTicks();
                return new ZonedDateTime(new Instant(ticks),
                                         new DeviceTimeZone());
            }
        }

        public Offset Offset { get { return _offset; } }

        public DateTimeZone Zone { get { return _zone ?? DateTimeZone.Utc; } }

        public LocalDateTime LocalDateTime { get { return _localDateTime; } }

        public CalendarSystem Calendar { get { return _localDateTime.Calendar; } }

        public LocalDate Date { get { return _localDateTime.Date; } }

        public Era Era { get { return _localDateTime.Era; } }

        internal Instant Instant { get { return _localDateTime.Instant; } }

        public LocalTime TimeOfDay { get { return _localDateTime.TimeOfDay; } }

        public int CenturyOfEra { get { return _localDateTime.CenturyOfEra; } }

        public int Year { get { return _localDateTime.Year; } }

        public int YearOfCentury { get { return _localDateTime.YearOfCentury; } }

        public int YearOfEra { get { return _localDateTime.YearOfEra; } }

        public int WeekYear { get { return _localDateTime.WeekYear; } }

        public int Month { get { return _localDateTime.Month; } }

        public int WeekOfWeekYear { get { return _localDateTime.WeekOfWeekYear; } }

        public int DayOfYear { get { return _localDateTime.DayOfYear; } }

        public int Day { get { return _localDateTime.Day; } }

        public IsoDayOfWeek IsoDayOfWeek { get { return _localDateTime.IsoDayOfWeek; } }

        public int DayOfWeek { get { return _localDateTime.DayOfWeek; } }

        public int Hour { get { return _localDateTime.Hour; } }

        public int ClockHourOfHalfDay { get { return _localDateTime.ClockHourOfHalfDay; } }

        public int Minute { get { return _localDateTime.Minute; } }

        public int Second { get { return _localDateTime.Second; } }

        public int Millisecond { get { return _localDateTime.Millisecond; } }

        public int TickOfSecond { get { return _localDateTime.TickOfSecond; } }

        public long TickOfDay { get { return _localDateTime.TickOfDay; } }

        public ZonedDateTime WithZone(DateTimeZone targetZone)
        {
            Preconditions.CheckNotNull(targetZone, "targetZone");
            return new ZonedDateTime(ToInstant(), targetZone, _localDateTime.Calendar);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, _localDateTime);
            hash = HashCodeHelper.Hash(hash, _offset);
            hash = HashCodeHelper.Hash(hash, _zone);
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ZonedDateTime)
            {
                return Equals((ZonedDateTime)obj);
            }
            return false;
        }

        public bool Equals(ZonedDateTime other)
        {
            return LocalDateTime == other.LocalDateTime && Offset == other.Offset && Zone.Equals(other.Zone);
        }

        public static bool operator ==(ZonedDateTime left, ZonedDateTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ZonedDateTime left, ZonedDateTime right)
        {
            return !(left == right);
        }

        public static bool operator <(ZonedDateTime lhs, ZonedDateTime rhs)
        {
            return lhs.ToInstant() < rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.Zone, rhs.Zone);
        }

        public static bool operator <=(ZonedDateTime lhs, ZonedDateTime rhs)
        {
            return lhs.ToInstant() <= rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.Zone, rhs.Zone);
        }

        public static bool operator >(ZonedDateTime lhs, ZonedDateTime rhs)
        {
            return lhs.ToInstant() > rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.Zone, rhs.Zone);
        }

        public static bool operator >=(ZonedDateTime lhs, ZonedDateTime rhs)
        {
            return lhs.ToInstant() >= rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.Zone, rhs.Zone);
        }

        public static ZonedDateTime operator +(ZonedDateTime zonedDateTime, Duration duration)
        {
            return new ZonedDateTime(zonedDateTime.ToInstant() + duration, zonedDateTime.Zone, zonedDateTime.LocalDateTime.Calendar);
        }

        public static ZonedDateTime Add(ZonedDateTime zonedDateTime, Duration duration)
        {
            return zonedDateTime + duration;
        }

        public ZonedDateTime Plus(Duration duration)
        {
            return this + duration;
        }

        public static ZonedDateTime Subtract(ZonedDateTime zonedDateTime, Duration duration)
        {
            return zonedDateTime - duration;
        }

        public ZonedDateTime Minus(Duration duration)
        {
            return this - duration;
        }

        public static ZonedDateTime operator -(ZonedDateTime zonedDateTime, Duration duration)
        {
            return new ZonedDateTime(zonedDateTime.ToInstant() - duration, zonedDateTime.Zone, zonedDateTime.LocalDateTime.Calendar);
        }

        public override string ToString()
        {
            return _localDateTime.ToString() + _zone.ToString() + _offset.ToString();
        }

        public OffsetDateTime ToOffsetDateTime()
        {
            return new OffsetDateTime(_localDateTime, _offset);
        }

        public Instant ToInstant()
        {
            return _localDateTime.Instant.Minus(_offset);
        }
    }
}
