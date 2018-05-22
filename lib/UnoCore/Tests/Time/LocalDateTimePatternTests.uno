using Uno;
using Uno.Testing;
using Uno.Time;
using Uno.Time.Text;

namespace Uno.Time.Test
{
    public class LocalDateTimePatternTests
    {
        [Test]
        public void Parse()
        {
            CheckParse(new LocalDateTime(2015, 7, 16, 19, 20, 0), "2015-07-16T19:20:00");

            CheckParse(new LocalDateTime(-15, 12, 1, 9, 2, 1), "-0015-12-01T09:02:01");
            CheckParse(new LocalDateTime(999, 11, 13, 07, 11, 11), "0999-11-13T07:11:11");
        }

        [Test]
        public void WrongFormat()
        {
            Assert.Throws<FormatException>(WrongEnding);
            Assert.Throws<FormatException>(IncorrectSeparator);
            Assert.Throws<FormatException>(MissingMinutes);
        }

        private void WrongEnding()
        {
            CheckParse(new LocalDateTime(2015, 7, 16, 19, 20, 0), "2015-07-16T19:20:00.");
        }

        private void IncorrectSeparator()
        {
            CheckParse(new LocalDateTime(2015, 7, 16, 19, 20, 0), "2015-07-16 19:20:");
        }

        private void MissingMinutes()
        {
            CheckParse(new LocalDateTime(2015, 7, 16, 19, 20, 0), "2015-07-16T19:2:20");
        }

        private void CheckParse(LocalDateTime expected, string text)
        {
            ParseResult<LocalDateTime> pr = LocalDateTimePattern.GeneralIsoPattern.Parse(text);
            Assert.AreEqual(expected, pr.Value);
        }
    }
}
