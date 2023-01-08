using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Uno.Build.Libraries
{
    class SearchPaths : List<string>
    {
        public void AddOnce(string path)
        {
            if (!Contains(path))
                Add(path);
        }

        public IEnumerable<DirectoryInfo> EnumerateLibraryDirectories(string name = "*")
        {
            foreach (var path in this)
            {
                var dir = new DirectoryInfo(path);
                if (dir.Exists)
                    foreach (var libDir in dir.EnumerateDirectories(name))
                        yield return libDir;
            }
        }

        public IEnumerable<DirectoryInfo> EnumerateVersionDirectories(string name = "*", string version = null)
        {
            version = version ?? "*";
            foreach (var lib in EnumerateLibraryDirectories(name)) {
                if (ManifestFile.Exists(lib.FullName))
                    yield return lib;
                else
                    foreach (var dir in lib.EnumerateDirectories(version))
                        if (ManifestFile.Exists(dir.FullName))
                            yield return dir;
            }
        }

        public DirectoryInfo[] GetOrderedVersionDirectories(string name = "*", string version = null)
        {
            var result = EnumerateVersionDirectories(name, version).ToArray();
            Array.Sort(result, (a, b) => -VersionRange.Compare(a.Name, b.Name));
            return result;
        }
    }
}