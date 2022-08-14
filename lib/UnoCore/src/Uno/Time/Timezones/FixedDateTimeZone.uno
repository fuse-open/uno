namespace Uno.Time
{
    internal sealed class FixedDateTimeZone : DateTimeZone
    {
        private readonly Offset offset;

        public FixedDateTimeZone(Offset offset) : this(UtcId, offset)
        {
        }

        public FixedDateTimeZone(string id, Offset offset) : base(id, true, offset, offset)
        {
            this.offset = offset;
        }

        public Offset Offset { get { return offset; } }

        public override Offset GetUtcOffset(LocalDateTime dateTime)
        {
            return offset;
        }

        public override string ToString()
        {
            return Id;
        }

        protected override bool EqualsImpl(DateTimeZone other)
        {
            FixedDateTimeZone otherZone = (FixedDateTimeZone) other;
            return offset == otherZone.offset && Id == other.Id;
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, offset);
            hash = HashCodeHelper.Hash(hash, Id);
            return hash;
        }
    }
}
