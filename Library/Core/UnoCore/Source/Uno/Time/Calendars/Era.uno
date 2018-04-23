namespace Uno.Time.Calendars
{
    public sealed class Era
    {
        public static readonly Era Common = new Era("CE");

        public static readonly Era BeforeCommon = new Era("BCE");

        public static readonly Era AnnoMartyrum = new Era("AM");

        public static readonly Era AnnoHegirae = new Era("EH");

        public static readonly Era AnnoMundi = new Era("AM");

        public static readonly Era AnnoPersico = new Era("AP");

        private readonly string _name;

        internal Era(string name)
        {
            _name = name;
        }

        public string Name { get { return _name; } }

        public override string ToString()
        {
            return _name;
        }
    }
}
