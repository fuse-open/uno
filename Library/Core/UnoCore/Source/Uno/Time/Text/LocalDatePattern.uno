namespace Uno.Time.Text
{
    public sealed class LocalDatePattern : IPattern<LocalDate>
    {
        private static LocalDatePattern General;

        public static LocalDatePattern GeneralIsoPattern
        {
            get
            {
                if (General == null)
                {
                    General = new LocalDatePattern();
                }
                return General;
            }
        }

        private FixedFormatPattern<LocalDateBucket> _generalPattern;

        private LocalDatePattern()
        {
            _generalPattern = new FixedFormatPattern<LocalDateBucket>(new IPatternPart<LocalDateBucket> []
                {
                    new SignPart<LocalDateBucket>(SetSign, GetSign),
                    new NumberPart<LocalDateBucket>(4, true, SetYear, GetYear),
                    new SeparatorPart<LocalDateBucket>(true, '-'),
                    new NumberPart<LocalDateBucket>(2, true, SetMonth, GetMonth),
                    new SeparatorPart<LocalDateBucket>(true, '-'),
                    new NumberPart<LocalDateBucket>(2, true, SetDay, GetDay),
                });
        }

        public ParseResult<LocalDate> Parse(string text)
        {
            try
            {
                var bucket = new LocalDateBucket();
                _generalPattern.Parse(text, bucket);

                var ld = new LocalDate(bucket.Sign*bucket.Year, bucket.Month, bucket.Day);
                return ParseResult<LocalDate>.ForValue(ld);
            }
            catch (Exception ex)
            {
                return ParseResult<LocalDate>.ForException(ex);
            }
        }

        public string Format(LocalDate value)
        {
            var bucket = new LocalDateBucket
                {
                    Sign = value.Year < 0 ? -1 : 1,
                    Year = value.Year,
                    Month = value.Month,
                    Day = value.Day,
                };
            return _generalPattern.Format(bucket);
        }

        private void SetSign(LocalDateBucket value, int sign)
        {
            value.Sign = sign;
        }

        private void SetYear(LocalDateBucket value, int year)
        {
            value.Year = year;
        }

        private void SetMonth(LocalDateBucket value, int month)
        {
            value.Month = month;
        }

        private void SetDay(LocalDateBucket value, int day)
        {
            value.Day = day;
        }

        private int GetSign(LocalDateBucket value)
        {
            return value.Sign;
        }

        private int GetYear(LocalDateBucket value)
        {
            return value.Year;
        }

        private int GetMonth(LocalDateBucket value)
        {
            return value.Month;
        }

        private int GetDay(LocalDateBucket value)
        {
            return value.Day;
        }

        class LocalDateBucket
        {
            public int Sign;
            public int Year;
            public int Month;
            public int Day;
        }
    }
}
