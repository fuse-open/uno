using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Build.Stuff;
using Uno.Compiler.API;
using Uno.Compiler.Frontend;
using Uno.IO;
using Uno.Logging;
using Log = Uno.Logging.Log;

namespace Uno.Build.Libraries
{
    public class LibraryDoctor : LogObject, IFrontendEnvironment
    {
        public LibraryDoctor(Log log)
            : base(log)
        {
        }

        public void Repair(List<string> optionalLibraries = null, bool force = false)
        {
            // Implicit --force when libraries are specified explicitly
            force = force || optionalLibraries?.Count > 0;

            foreach (var dir in EnumerateDirectories(optionalLibraries))
            {
                var path = dir.FullName;

                try
                {
                    // Skip locally built libraries (taken care of by LibraryBuilder)
                    if (ManifestFile.Exists(path) &&
                            !File.Exists(Path.Combine(path, ".unobuild")) &&
                            Repair(ManifestFile.Load(path), force))
                        Log.Message("Updated " + path.ToRelativePath().Quote());
                }
                catch (Exception e)
                {
                    Log.Trace(e);
                    Log.Error("Failed to load library " + path.ToRelativePath().Quote() + ": " + e.Message);
                }
            }
        }

        public bool Repair(ManifestFile file, bool force = false)
        {
            var bundle = file.CreateBundle();
            var reader = new SourceReader(Log, bundle, this);

            if (force || !reader.CacheExists ||
                reader.HasAnythingChangedSince(reader.CacheTime, false))
            {
                Log.Verbose("Generating cache for " + file.Name);
                using (new FileLock(Log, file.CacheDirectory))
                    reader.ExportCache(file.CacheDirectory);
                return true;
            }

            return false;
        }

        public IEnumerable<DirectoryInfo> EnumerateDirectories(List<string> optionalPackages = null)
        {
            var cache = new BundleCache();

            if (optionalPackages == null || optionalPackages.Count == 0)
                foreach (var dir in cache.EnumerateVersions("*"))
                    yield return dir;
            else
            {
                foreach (var p in optionalPackages)
                {
                    var result = cache.EnumerateVersions(p).ToArray();
                    if (result.Length == 0)
                        Log.Warning("Package " + p.Quote() + " was not found");
                    else
                        foreach (var dir in result)
                            yield return dir;
                }
            }
        }

        bool IFrontendEnvironment.CanCacheIL => true;
        bool IFrontendEnvironment.Parallel => true;
        bool IFrontendEnvironment.Test(Source src, string optionalCond) => true;
    }
}
