using System.Collections.Generic;
using System.Linq;

namespace Uno.Build.Packages.Feeds
{
    public class CacheFeed : IPackageFeed
    {
        public IEnumerable<IPackage> FindPackages(IReadOnlyList<string> names, string version = null)
        {
            var cache = new PackageCache();
            var list = new List<LocalPackage>();
            var namesUpper = new HashSet<string>(names.Select(x => x.ToUpperInvariant()));
            var versionUpper = string.IsNullOrEmpty(version) ? null : version.ToUpperInvariant();

            foreach (var directory in cache.SearchPaths)
                foreach (var package in cache.GetPackageDirectories(directory).Keys)
                    if (namesUpper.Count == 0 || namesUpper.Contains(package.ToUpperInvariant()))
                        foreach (var versionDir in cache.GetVersionDirectories(package))
                            if ((versionUpper == null || versionUpper == versionDir.Name.ToUpperInvariant()) &&
                                PackageFile.Exists(versionDir.FullName))
                                list.Add(new LocalPackage(package, versionDir.Name, versionDir.FullName));
            list.Sort();
            return list;
        }

        public override string ToString()
        {
            return "(cache-feed)";
        }
    }
}