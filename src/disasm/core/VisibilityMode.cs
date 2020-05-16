namespace Uno.Disasm
{
    public struct VisibilityMode
    {
        public static readonly VisibilityMode[] Items =
        {
            new VisibilityMode("Project Only, Public Only", VisibilityFlags.ProjectOnly | VisibilityFlags.PublicOnly),
            new VisibilityMode("Project Only, All Members", VisibilityFlags.ProjectOnly),
            new VisibilityMode("All Packages, Public Only", VisibilityFlags.PublicOnly),
            new VisibilityMode("All Packages, All Members", VisibilityFlags.None),
        };

        public readonly string Name;
        public readonly VisibilityFlags Flags;

        public VisibilityMode(string name, VisibilityFlags flags)
        {
            Name = name;
            Flags = flags;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
