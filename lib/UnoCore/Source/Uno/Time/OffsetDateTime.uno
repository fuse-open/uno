using Uno.Time.Text;
using Uno.Time.Calendars;

namespace Uno.Time
{
    public struct OffsetDateTime
    {
        private readonly LocalDateTime _localDateTime;
        private readonly Offset _offset;

        public OffsetDateTime(LocalDateTime localDateTime, Offset offset)
        {
            _localDateTime = localDateTime;
            _offset = offset;
        }

        public CalendarSystem Calendar { get { return _localDateTime.Calendar; } }

        public int Year { get { return _localDateTime.Year; } }

        public int Month { get { return _localDateTime.Month; } }

        public int Day { get { return _localDateTime.Day; } }

        public IsoDayOfWeek IsoDayOfWeek { get { return _localDateTime.IsoDayOfWeek; } }

        public int DayOfWeek { get { return _localDateTime.DayOfWeek; } }

        public int WeekYear { get { return _localDateTime.WeekYear; } }

        public int WeekOfWeekYear { get { return _localDateTime.WeekOfWeekYear; } }

        public int YearOfCentury { get { return _localDateTime.YearOfCentury; } }

        public int YearOfEra { get { return _localDateTime.YearOfEra; } }

        public Era Era { get { return _localDateTime.Era; } }

        public int DayOfYear { get { return _localDateTime.DayOfYear; } }

        public int Hour { get { return _localDateTime.Hour;  } }

        public int ClockHourOfHalfDay { get { return _localDateTime.ClockHourOfHalfDay; } }

        public int Minute { get { return _localDateTime.Minute; } }

        public int Second { get { return _localDateTime.Second; } }

        public int Millisecond { get { return _localDateTime.Millisecond; } }

        public int TickOfSecond { get { return _localDateTime.TickOfSecond; } }

        public long TickOfDay { get { return _localDateTime.TickOfDay; } }

        public LocalDateTime LocalDateTime { get { return _localDateTime; } }

        public LocalDate Date { get { return _localDateTime.Date; } }

        public LocalTime TimeOfDay { get { return _localDateTime.TimeOfDay; } }

        public Offset Offset { get { return _offset; } }

        public Instant ToInstant()
        {
            return _localDateTime.Instant.Minus(_offset);
        }

        public OffsetDateTime WithOffset(Offset offset)
        {
            LocalDateTime newLocalDateTime = new LocalDateTime(LocalDateTime.Instant.Minus(this.Offset).Plus(offset), Calendar);
            return new OffsetDateTime(newLocalDateTime, offset);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, _localDateTime);
            hash = HashCodeHelper.Hash(hash, _offset);
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is OffsetDateTime)
            {
                return this == (OffsetDateTime)obj;
            }
            return false;
        }

        public bool Equals(OffsetDateTime other)
        {
            return this._localDateTime == other._localDateTime && _offset == other._offset;
        }

        public override string ToString()
        {
            return OffsetDateTimePattern.GeneralIsoPattern.Format(this);
        }

        public static bool operator ==(OffsetDateTime left, OffsetDateTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OffsetDateTime left, OffsetDateTime right)
        {
            return !(left == right);
        }

        public ZonedDateTime InFixedZone()
        {
            return new ZonedDateTime(_localDateTime, _offset, DateTimeZone.ForOffset(_offset));
        }
    }
}
