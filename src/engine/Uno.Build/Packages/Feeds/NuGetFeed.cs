using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using NuGet;
using Uno.Logging;

namespace Uno.Build.Packages.Feeds
{
    public class NuGetFeed : LogObject, IPackageFeed
    {
        readonly IPackageRepository _repo;

        public NuGetFeed(Log log, string source)
            : base(log)
        {
            // Download cache location -- don't collide with NuGet's own cache!
            Environment.SetEnvironmentVariable("NuGetCachePath",
                PackageManager.CacheDir);

            _repo = PackageRepositoryFactory
                    .Default
                    .CreateRepository(source);
        }

        IList<T> Retry<T>(Func<IEnumerable<T>> func, int maxRetries)
        {
            int failures = 0;
            while (true)
            {
                try
                {
                    return func().ToList();
                }
                catch (Exception e)
                {
                    failures++;
                    if (failures == maxRetries)
                        throw;

                    Log.Trace(e);
                    Log.Warning("Failed to find package, retrying: " + e.Message);
                    Thread.Sleep(100);
                }
            }
        }

        public IEnumerable<IPackage> FindPackages(IReadOnlyList<string> names, string version = null)
        {
            if (names.Count == 0 && string.IsNullOrEmpty(version))
            {
                var counter = 0;
                foreach (var p in _repo.GetPackages())
                {
                    yield return new NuGetPackage(Log, _repo, p);

                    // Abort after 100 packages, otherwise it could take too much time
                    if (++counter == 100)
                    {
                        Log.Warning(_repo.Source + ": Too many packages");
                        yield break;
                    }
                }
            }
            else if (string.IsNullOrEmpty(version))
            {
                foreach (var p in Retry(() => _repo.FindPackages(names), 5))
                    yield return new NuGetPackage(Log, _repo, p);
            }
            else
            {
                var spec = new VersionSpec(SemanticVersion.Parse(version));
                foreach (var f in names)
                    foreach (var p in Retry(() => _repo.FindPackages(f, spec, true, true), 5))
                        yield return new NuGetPackage(Log, _repo, p);
            }
        }

        public override string ToString()
        {
            return _repo.Source;
        }
    }
}
