using Uno;
using Uno.Testing;
using Uno.Time;
using Uno.Time.Text;

namespace Uno.Time.Test
{
    public class OffsetDateTimePatternTests
    {
        [Test]
        public void Parse()
        {
            CheckParse(new OffsetDateTime(new LocalDateTime(2015, 7, 16, 19, 20, 0), Offset.FromHours(1)), "2015-07-16T19:20:00+01:00:00");
            CheckParse(new OffsetDateTime(new LocalDateTime(-15, 12, 1, 9, 2, 1), Offset.FromHours(1)), "-0015-12-01T09:02:01+01:00");
            CheckParse(new OffsetDateTime(new LocalDateTime(-15, 12, 1, 9, 2, 1), Offset.FromHoursAndMinutes(-2, -52)), "-0015-12-01T09:02:01-02:52");
            CheckParse(new OffsetDateTime(new LocalDateTime(999, 11, 13, 07, 11, 11), Offset.FromHours(1)), "0999-11-13T07:11:11+01");
            CheckParse(new OffsetDateTime(new LocalDateTime(999, 11, 13, 07, 11, 11), Offset.Zero), "0999-11-13T07:11:11Z");
            CheckParse(new OffsetDateTime(new LocalDateTime(999, 11, 13, 07, 11, 11, 123), Offset.Zero), "0999-11-13T07:11:11.123Z");
            CheckParse(new OffsetDateTime(new LocalDateTime(999, 11, 13, 07, 11, 11, 123),  Offset.FromHours(1)), "0999-11-13T07:11:11.123+01:00:00");
        }

        [Test]
        public void WrongFormat()
        {
            Assert.Throws<FormatException>(WrongEnding);
            Assert.Throws<FormatException>(IncorrectSeparator);
            Assert.Throws<FormatException>(MissingSeconds);
        }

        private void WrongEnding()
        {
            CheckParse(new OffsetDateTime(new LocalDateTime(2015, 7, 16, 19, 20, 0), Offset.FromHours(1)), "2015-07-16T19:20:00+01:00:");
        }

        private void IncorrectSeparator()
        {
            CheckParse(new OffsetDateTime(new LocalDateTime(2015, 7, 16, 19, 20, 0), Offset.FromHours(1)), "2015-07-16 19:20:00+01:00:00");
        }

        private void MissingSeconds()
        {
            CheckParse(new OffsetDateTime(new LocalDateTime(2015, 7, 16, 19, 20, 0), Offset.FromHours(1)), "2015-07-16T19:20+01:00:00");
        }

        private void CheckParse(OffsetDateTime expected, string text)
        {
            ParseResult<OffsetDateTime> pr = OffsetDateTimePattern.GeneralIsoPattern.Parse(text);
            Assert.AreEqual(expected, pr.Value);
        }
    }
}
