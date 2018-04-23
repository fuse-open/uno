using System.Collections.Generic;

namespace Uno.Build.Packages
{
    public interface IPackageFeed
    {
        IEnumerable<IPackage> FindPackages(IReadOnlyList<string> names, string version = null);
    }
}