using Uno;
using Uno.Testing;

namespace UnoTest.Issues
{
    class DateTimeConverterHelpers
    {
        public static DateTime ConvertDateToDateTime(int ticks)
        {
            return new DateTime(ticks, DateTimeKind.Utc);
        }
    }

    public class Issue1406
    {
        [Test]
        public void Main()
        {
            var tmp = DateTimeConverterHelpers.ConvertDateToDateTime(0);
        }
    }
}
