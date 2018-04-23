using Uno;
using Uno.Testing;
using Uno.Time;
using Uno.Time.Text;

namespace Uno.Time.Test
{
    public class LocalTimePatternTests
    {
        [Test]
        public void Parse()
        {
            CheckParse(new LocalTime(19, 20, 0), "19:20:00");
            CheckParse(new LocalTime(9, 2, 13, 241), "09:02:13.241");
            CheckParse(new LocalTime(9, 2, 0, 160), "09:02:00.16");
            CheckParse(new LocalTime(9, 2, 0, 1), "09:02:00.0016");
            CheckParse(new LocalTime(7, 11, 11), "07:11:11");
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
            CheckParse(new LocalTime(19, 20, 0), "19:20:00.");
        }

        private void IncorrectSeparator()
        {
            CheckParse(new LocalTime(19, 20, 0), "19-20");
        }

        private void MissingMinutes()
        {
            CheckParse(new LocalTime(19, 20, 0), "19:2:20");
        }

        private void CheckParse(LocalTime expected, string text)
        {
            ParseResult<LocalTime> pr = LocalTimePattern.GeneralIsoPattern.Parse(text);
            Assert.AreEqual(expected, pr.Value);
        }
    }
}
