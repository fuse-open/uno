using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Uno.Build.Packages
{
    class PackageSearchPaths : List<string>
    {
        public void AddOnce(string path)
        {
            if (!Contains(path))
                Add(path);
        }

        public IEnumerable<DirectoryInfo> EnumeratePackageDirectories(string name = "*")
        {
            foreach (var path in this)
            {
                var dir = new DirectoryInfo(path);
                if (dir.Exists)
                    foreach (var packageDir in dir.EnumerateDirectories(name))
                        yield return packageDir;
            }
        }

        public IEnumerable<DirectoryInfo> EnumerateVersionDirectories(string name = "*", string version = null)
        {
            version = version ?? "*";
            foreach (var package in EnumeratePackageDirectories(name))
                foreach (var dir in package.EnumerateDirectories(version))
                    if (PackageFile.Exists(dir.FullName))
                        yield return dir;
        }

        public DirectoryInfo[] GetOrderedVersionDirectories(string name = "*", string version = null)
        {
            var result = EnumerateVersionDirectories(name, version).ToArray();
            Array.Sort(result, (a, b) => -VersionRange.Compare(a.Name, b.Name));
            return result;
        }
    }
}