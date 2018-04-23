namespace Uno.Time
{
    public abstract class DateTimeZone
    {
        internal const string UtcId = "UTC";

        private readonly string id;
        private readonly bool isFixed;

        private readonly long minOffsetTicks;
        private readonly long maxOffsetTicks;

        protected DateTimeZone(string id, bool isFixed, Offset minOffset, Offset maxOffset)
        {
            this.id = id;
            this.isFixed = isFixed;
            this.minOffsetTicks = minOffset.Ticks;
            this.maxOffsetTicks = maxOffset.Ticks;
        }

        private static readonly DateTimeZone UtcZone = new FixedDateTimeZone(Offset.Zero);

        public static DateTimeZone Utc { get { return UtcZone; } }

        public string Id { get { return id; } }
        internal bool IsFixed { get { return isFixed; } }

        public Offset MinOffset { get { return Offset.FromTicks(minOffsetTicks); } }
        public Offset MaxOffset { get { return Offset.FromTicks(maxOffsetTicks); } }

        public ZonedDateTime AtStrictly(LocalDateTime localDateTime)
        {
            return new ZonedDateTime(localDateTime, this);
        }

        public static DateTimeZone ForOffset(Offset offset)
        {
            return new FixedDateTimeZone(offset);
        }

        public abstract Offset GetUtcOffset(LocalDateTime dateTime);

        public override string ToString()
        {
            return Id;
        }

        public override sealed bool Equals(object obj)
        {
            return Equals(obj as DateTimeZone);
        }

        public bool Equals(DateTimeZone obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return !ReferenceEquals(obj, null) && obj.GetType() == GetType() && EqualsImpl(obj);
        }

        public override int GetHashCode()
        {
            // Silence warning
            // TODO: This won't actually be consistent without 'protected abstract GetHashCodeImpl()' (design flaw)
            return base.GetHashCode();
        }

        protected abstract bool EqualsImpl(DateTimeZone zone);
    }
}
