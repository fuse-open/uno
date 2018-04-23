using Uno.Diagnostics;

namespace Uno.Time
{
    public class DeviceTimeZone : DateTimeZone
    {
        public DeviceTimeZone() : this(UtcId)
        {
        }

        internal DeviceTimeZone(string id)
            : base(id, false, Offset.FromHours(-12), Offset.FromHours(12))
        {
        }

        public override Offset GetUtcOffset(LocalDateTime dateTime)
        {
            var offsetMinutes = Clock.GetTimezoneOffset(dateTime.Year, dateTime.Month,
                                                        dateTime.Day);
            return Offset.FromHoursAndMinutes(offsetMinutes / Constants.MinutesPerHour,
                                              offsetMinutes % Constants.MinutesPerHour);
        }

        public override string ToString()
        {
            return Id;
        }

        protected override bool EqualsImpl(DateTimeZone other)
        {
            return other is DeviceTimeZone;
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Id);
            return hash;
        }
    }
}
