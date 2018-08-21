namespace Uno.Time.Text
{
    public sealed class OffsetDateTimePattern : IPattern<OffsetDateTime>
    {
        private static OffsetDateTimePattern General;

        public static OffsetDateTimePattern GeneralIsoPattern
        {
            get
            {
                if (General == null)
                {
                    General = new OffsetDateTimePattern();
                }
                return General;
            }
        }

        private FixedFormatPattern<OffsetDateTimeBucket> _generalPattern;

        private OffsetDateTimePattern()
        {
            _generalPattern = new FixedFormatPattern<OffsetDateTimeBucket>(new IPatternPart<OffsetDateTimeBucket> []
                {
                    new SignPart<OffsetDateTimeBucket>(SetSign, GetSign),
                    new NumberPart<OffsetDateTimeBucket>(4, true, SetYear, GetYear),
                    new SeparatorPart<OffsetDateTimeBucket>(true, '-'),
                    new NumberPart<OffsetDateTimeBucket>(2, true, SetMonth, GetMonth),
                    new SeparatorPart<OffsetDateTimeBucket>(true, '-'),
                    new NumberPart<OffsetDateTimeBucket>(2, true, SetDay, GetDay),
                    new SeparatorPart<OffsetDateTimeBucket>(true, 'T'),
                    new NumberPart<OffsetDateTimeBucket>(2, true, SetHour, GetHour),
                    new SeparatorPart<OffsetDateTimeBucket>(true, ':'),
                    new NumberPart<OffsetDateTimeBucket>(2, true, SetMinute, GetMinute),
                    new SeparatorPart<OffsetDateTimeBucket>(true, ':'),
                    new NumberPart<OffsetDateTimeBucket>(2, true, SetSecond, GetSecond),
                    new SeparatorPart<OffsetDateTimeBucket>(false, '.', 1),
                    new RangeNumberPart<OffsetDateTimeBucket>(3, 3, SetMillisecond, GetMillisecond),

                    new OffsetPatternPart<OffsetDateTimeBucket>(SetOffset, GetOffset)
                });
        }

        public ParseResult<OffsetDateTime> Parse(string text)
        {
            try
            {
                var bucket = new OffsetDateTimeBucket();
                _generalPattern.Parse(text, bucket);

                var ldt = new LocalDateTime(bucket.Sign*bucket.Year, bucket.Month, bucket.Day,
                                  bucket.Hour, bucket.Minute, bucket.Second, bucket.Millisecond);
                var result = new OffsetDateTime(ldt, bucket.Offset);
                return ParseResult<OffsetDateTime>.ForValue(result);
            }
            catch (Exception ex)
            {
                return ParseResult<OffsetDateTime>.ForException(ex);
            }
        }

        public string Format(OffsetDateTime value)
        {
            var bucket = new OffsetDateTimeBucket
                {
                    Sign = value.Year < 0 ? -1 : 1,
                    Year = value.Year,
                    Month = value.Month,
                    Day = value.Day,
                    Hour = value.Hour,
                    Minute = value.Minute,
                    Second = value.Second,
                    Millisecond = value.Millisecond,
                    Offset = value.Offset
                };
            return _generalPattern.Format(bucket);
        }

        private void SetSign(OffsetDateTimeBucket value, int sign)
        {
            value.Sign = sign;
        }

        private int GetSign(OffsetDateTimeBucket value)
        {
            return value.Sign;
        }

        private void SetYear(OffsetDateTimeBucket value, int year)
        {
            value.Year = year;
        }

        private int GetYear(OffsetDateTimeBucket value)
        {
            return value.Year;
        }

        private void SetMonth(OffsetDateTimeBucket value, int month)
        {
            value.Month = month;
        }

        private int GetMonth(OffsetDateTimeBucket value)
        {
            return value.Month;
        }

        private void SetDay(OffsetDateTimeBucket value, int day)
        {
            value.Day = day;
        }

        private int GetDay(OffsetDateTimeBucket value)
        {
            return value.Day;
        }

        private void SetHour(OffsetDateTimeBucket value, int hour)
        {
            value.Hour = hour;
        }

        private int GetHour(OffsetDateTimeBucket value)
        {
            return value.Hour;
        }

        private void SetMinute(OffsetDateTimeBucket value, int minute)
        {
            value.Minute = minute;
        }

        private int GetMinute(OffsetDateTimeBucket value)
        {
            return value.Minute;
        }

        private void SetSecond(OffsetDateTimeBucket value, int second)
        {
            value.Second = second;
        }

        private int GetSecond(OffsetDateTimeBucket value)
        {
            return value.Second;
        }

        private void SetMillisecond(OffsetDateTimeBucket value, int millisecond)
        {
            value.Millisecond = millisecond;
        }

        private int GetMillisecond(OffsetDateTimeBucket value)
        {
            return value.Millisecond;
        }

        private void SetOffset(OffsetDateTimeBucket value, Offset offset)
        {
            value.Offset = offset;
        }

        private Offset GetOffset(OffsetDateTimeBucket value)
        {
            return value.Offset;
        }

        class OffsetDateTimeBucket
        {
            public int Sign;
            public int Year;
            public int Month;
            public int Day;
            public int Hour;
            public int Minute;
            public int Second;
            public int Millisecond;

            public Offset Offset;
        }
    }
}
