using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Uno.Build.Stuff
{
    static class LongPathZipFile
    {
        public static void ExtractToDirectory(string zipFile, string targetDir)
        {
            var dirsCreated = new HashSet<string>();
            var archive = ZipFile.Open(zipFile, ZipArchiveMode.Read);
            foreach (var entry in archive.Entries)
            {
                var fullName = entry.FullName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                foreach (var dirsToCreate in FindDirectoriesToCreate(dirsCreated, fullName))
                {
                    LongPathDisk.CreateDirectory(Path.Combine(targetDir, dirsToCreate));
                    dirsCreated.Add(dirsToCreate);
                }

                if (fullName.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    continue; // Continue when entry is a directory
                
                using (MemoryStream ms = new MemoryStream())
                {
                    entry.Open().CopyTo(ms);
                    LongPathDisk.WriteAllBytes(Path.Combine(targetDir, fullName), ms.ToArray());
                }
            }
        }

        static IEnumerable<string> FindDirectoriesToCreate(HashSet<string> paths, string search)
        {
            var path = Path.GetDirectoryName(search);
            if (string.IsNullOrEmpty(path))
                yield break;

            if (!paths.Contains(path))
            {
                foreach (var d in FindDirectoriesToCreate(paths, path))
                    yield return d;
                yield return path;
            }
        }
    }
}
