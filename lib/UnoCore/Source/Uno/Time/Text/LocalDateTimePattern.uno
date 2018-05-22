namespace Uno.Time.Text
{
    public sealed class LocalDateTimePattern : IPattern<LocalDateTime>
    {
        private static LocalDateTimePattern General;

        public static LocalDateTimePattern GeneralIsoPattern
        {
            get
            {
                if (General == null)
                {
                    General = new LocalDateTimePattern();
                }
                return General;
            }
        }

        private FixedFormatPattern<LocalDateTimeBucket> _generalPattern;

        private LocalDateTimePattern()
        {
            _generalPattern = new FixedFormatPattern<LocalDateTimeBucket>(new IPatternPart<LocalDateTimeBucket> []
                {
                    new SignPart<LocalDateTimeBucket>(SetSign, GetSign),
                    new NumberPart<LocalDateTimeBucket>(4, true, SetYear, GetYear),
                    new SeparatorPart<LocalDateTimeBucket>(true, '-'),
                    new NumberPart<LocalDateTimeBucket>(2, true, SetMonth, GetMonth),
                    new SeparatorPart<LocalDateTimeBucket>(true, '-'),
                    new NumberPart<LocalDateTimeBucket>(2, true, SetDay, GetDay),
                    new SeparatorPart<LocalDateTimeBucket>(true, 'T'),
                    new NumberPart<LocalDateTimeBucket>(2, true, SetHour, GetHour),
                    new SeparatorPart<LocalDateTimeBucket>(true, ':'),
                    new NumberPart<LocalDateTimeBucket>(2, true, SetMinute, GetMinute),
                    new SeparatorPart<LocalDateTimeBucket>(true, ':'),
                    new NumberPart<LocalDateTimeBucket>(2, true, SetSecond, GetSecond),
                });
        }

        public ParseResult<LocalDateTime> Parse(string text)
        {
            try
            {
                var bucket = new LocalDateTimeBucket();
                _generalPattern.Parse(text, bucket);

                var ldt = new LocalDateTime(bucket.Sign*bucket.Year, bucket.Month, bucket.Day,
                                  bucket.Hour, bucket.Minute, bucket.Second);
                return ParseResult<LocalDateTime>.ForValue(ldt);
            }
            catch (Exception ex)
            {
                return ParseResult<LocalDateTime>.ForException(ex);
            }
        }

        public string Format(LocalDateTime value)
        {
            var bucket = new LocalDateTimeBucket
                {
                    Sign = value.Year < 0 ? -1 : 1,
                    Year = value.Year,
                    Month = value.Month,
                    Day = value.Day,
                    Hour = value.Hour,
                    Minute = value.Minute,
                    Second = value.Second,
                };
            return _generalPattern.Format(bucket);
        }

        private void SetSign(LocalDateTimeBucket value, int sign)
        {
            value.Sign = sign;
        }

        private int GetSign(LocalDateTimeBucket value)
        {
            return value.Sign;
        }

        private void SetYear(LocalDateTimeBucket value, int year)
        {
            value.Year = year;
        }

        private int GetYear(LocalDateTimeBucket value)
        {
            return value.Year;
        }

        private void SetMonth(LocalDateTimeBucket value, int month)
        {
            value.Month = month;
        }

        private int GetMonth(LocalDateTimeBucket value)
        {
            return value.Month;
        }

        private void SetDay(LocalDateTimeBucket value, int day)
        {
            value.Day = day;
        }

        private int GetDay(LocalDateTimeBucket value)
        {
            return value.Day;
        }

        private void SetHour(LocalDateTimeBucket value, int hour)
        {
            value.Hour = hour;
        }

        private int GetHour(LocalDateTimeBucket value)
        {
            return value.Hour;
        }

        private void SetMinute(LocalDateTimeBucket value, int minute)
        {
            value.Minute = minute;
        }

        private int GetMinute(LocalDateTimeBucket value)
        {
            return value.Minute;
        }

        private void SetSecond(LocalDateTimeBucket value, int second)
        {
            value.Second = second;
        }

        private int GetSecond(LocalDateTimeBucket value)
        {
            return value.Second;
        }

        class LocalDateTimeBucket
        {
            public int Sign;
            public int Year;
            public int Month;
            public int Day;
            public int Hour;
            public int Minute;
            public int Second;
        }
    }
}
