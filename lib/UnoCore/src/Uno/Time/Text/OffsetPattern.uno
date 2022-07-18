namespace Uno.Time.Text
{
    public sealed class OffsetPattern : IPattern<Offset>
    {
        private static OffsetPattern General;

        public static OffsetPattern GeneralIsoPattern
        {
            get
            {
                if (General == null)
                {
                    General = new OffsetPattern();
                }
                return General;
            }
        }

        private FixedFormatPattern<OffsetBucket> _generalPattern;

        private OffsetPattern()
        {
            _generalPattern = new FixedFormatPattern<OffsetBucket>(new IPatternPart<OffsetBucket> []
                {
                    new SignPart<OffsetBucket>(true, SetSign, GetSign),
                    new NumberPart<OffsetBucket>(2, true, SetHour, GetHour),
                    new SeparatorPart<OffsetBucket>(false, true, ':'),
                    new NumberPart<OffsetBucket>(2, false, SetMinute, GetMinute),
                    new SeparatorPart<OffsetBucket>(false, true, ':'),
                    new NumberPart<OffsetBucket>(2, false, SetSecond, GetSecond),
                });
        }

        public ParseResult<Offset> Parse(string text)
        {
            try
            {
                if (text == "Z")
                    return ParseResult<Offset>.ForValue(Offset.Zero);

                var bucket = new OffsetBucket();
                _generalPattern.Parse(text, bucket);

                var offset = Offset.FromHoursAndMinutes(bucket.Sign*bucket.Hour,
                                                        bucket.Sign*bucket.Minute);
                return ParseResult<Offset>.ForValue(offset);
            }
            catch (Exception ex)
            {
                return ParseResult<Offset>.ForException(ex);
            }
        }

        public string Format(Offset value)
        {
            var bucket = new OffsetBucket();
            bucket.Sign = value.Milliseconds < 0 ? -1 : 1;
            bucket.Hour = value.Milliseconds / Constants.MillisecondsPerHour;
            int remaining = value.Milliseconds % Constants.MillisecondsPerHour;
            bucket.Minute = remaining / Constants.MillisecondsPerMinute;
            remaining = remaining % Constants.MillisecondsPerMinute;
            bucket.Second = remaining / Constants.MillisecondsPerSecond;
            return _generalPattern.Format(bucket);
        }

        private void SetSign(OffsetBucket value, int sign)
        {
            value.Sign = sign;
        }

        private int GetSign(OffsetBucket value)
        {
            return value.Sign;
        }

        private void SetHour(OffsetBucket value, int hour)
        {
            value.Hour = hour;
        }

        private int GetHour(OffsetBucket value)
        {
            return value.Hour;
        }

        private void SetMinute(OffsetBucket value, int minute)
        {
            value.Minute = minute;
        }

        private int GetMinute(OffsetBucket value)
        {
            return value.Minute;
        }

        private void SetSecond(OffsetBucket value, int second)
        {
            value.Second = second;
        }

        private int GetSecond(OffsetBucket value)
        {
            return value.Second;
        }

        class OffsetBucket
        {
            public int Sign;
            public int Hour;
            public int Minute;
            public int Second;
        }
    }
}
