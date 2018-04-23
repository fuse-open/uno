namespace Uno.Build.Packages
{
    public interface IPackage
    {
        string Name { get; }
        string Version { get; }
        string Source { get; }

        void Install(string directory);
    }
}