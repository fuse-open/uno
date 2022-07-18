using Uno;
using Uno.Text;
using Uno.Testing;
using Uno.Time;

namespace Uno.Time.Test
{
    public class DeviceTimeZoneMock : DateTimeZone
    {
        public DeviceTimeZoneMock()
            : base("UTC", false, Offset.FromHours(-12), Offset.FromHours(12))
        {
        }

        public override Offset GetUtcOffset(LocalDateTime dateTime)
        {
            if (dateTime >= new LocalDateTime(2008, 3, 1, 0, 0))
                return Offset.FromHours(4);
            else
                return Offset.FromHours(3);
        }

        protected override bool EqualsImpl(DateTimeZone other)
        {
            return other is DeviceTimeZoneMock;
        }
    }
}
