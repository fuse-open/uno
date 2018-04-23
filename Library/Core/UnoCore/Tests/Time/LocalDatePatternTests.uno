using Uno;
using Uno.Testing;
using Uno.Time;
using Uno.Time.Text;

namespace Uno.Time.Test
{
    public class LocalDatePatternTests
    {
        [Test]
        public void Parse()
        {
            CheckParse(new LocalDate(2015, 7, 16), "2015-07-16");
            CheckParse(new LocalDate(-15, 12, 1), "-0015-12-01");
            CheckParse(new LocalDate(999, 11, 13), "0999-11-13");
        }

        [Test]
        public void WrongFormat()
        {
            Assert.Throws<FormatException>(WrongEnding);
            Assert.Throws<FormatException>(IncorrectSeparator);
            Assert.Throws<FormatException>(MissingDays);
        }

        private void WrongEnding()
        {
            CheckParse(new LocalDate(2015, 7, 16), "2015-07-");
        }

        private void IncorrectSeparator()
        {
            CheckParse(new LocalDate(2015, 7, 16), "2015/07/16");
        }

        private void MissingDays()
        {
            CheckParse(new LocalDate(2015, 7, 16), "2015-07-1");
        }

        private void CheckParse(LocalDate expected, string text)
        {
            ParseResult<LocalDate> pr = LocalDatePattern.GeneralIsoPattern.Parse(text);
            Assert.AreEqual(expected, pr.Value);
        }
    }
}
