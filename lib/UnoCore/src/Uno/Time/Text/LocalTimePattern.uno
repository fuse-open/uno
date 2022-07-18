namespace Uno.Time.Text
{
    public sealed class LocalTimePattern : IPattern<LocalTime>
    {
        private static LocalTimePattern General;

        public static LocalTimePattern GeneralIsoPattern
        {
            get
            {
                if (General == null)
                {
                    General = new LocalTimePattern();
                }
                return General;
            }
        }

        private FixedFormatPattern<LocalTimeBucket> _generalPattern;

        private LocalTimePattern()
        {
            _generalPattern = new FixedFormatPattern<LocalTimeBucket>(new IPatternPart<LocalTimeBucket> []
                {
                    new NumberPart<LocalTimeBucket>(2, true, SetHour, GetHour),
                    new SeparatorPart<LocalTimeBucket>(true, ':'),
                    new NumberPart<LocalTimeBucket>(2, true, SetMinute, GetMinute),
                    new SeparatorPart<LocalTimeBucket>(true, ':'),
                    new NumberPart<LocalTimeBucket>(2, true, SetSecond, GetSecond),
                    new SeparatorPart<LocalTimeBucket>(false, '.'),
                    new RangeNumberPart<LocalTimeBucket>(3, 7, SetMillisecond, GetMillisecond)
                });
        }

        public ParseResult<LocalTime> Parse(string text)
        {
            try
            {
                var bucket = new LocalTimeBucket();
                _generalPattern.Parse(text, bucket);

                var lt = new LocalTime(bucket.Hour, bucket.Minute, bucket.Second, bucket.Millisecond);
                return ParseResult<LocalTime>.ForValue(lt);
            }
            catch (Exception ex)
            {
                return ParseResult<LocalTime>.ForException(ex);
            }
        }

        public string Format(LocalTime value)
        {
            var bucket = new LocalTimeBucket
                {
                    Hour = value.Hour,
                    Minute = value.Minute,
                    Second = value.Second,
                    Millisecond = value.Millisecond
                };
            return _generalPattern.Format(bucket);
        }

        private void SetHour(LocalTimeBucket value, int hour)
        {
            value.Hour = hour;
        }

        private int GetHour(LocalTimeBucket value)
        {
            return value.Hour;
        }

        private void SetMinute(LocalTimeBucket value, int minute)
        {
            value.Minute = minute;
        }

        private int GetMinute(LocalTimeBucket value)
        {
            return value.Minute;
        }

        private void SetSecond(LocalTimeBucket value, int second)
        {
            value.Second = second;
        }

        private int GetSecond(LocalTimeBucket value)
        {
            return value.Second;
        }

        private void SetMillisecond(LocalTimeBucket value, int millisecond)
        {
            value.Millisecond = millisecond;
        }

        private int GetMillisecond(LocalTimeBucket value)
        {
            return value.Millisecond;
        }

        class LocalTimeBucket
        {
            public int Hour;
            public int Minute;
            public int Second;
            public int Millisecond;
        }
    }
}
