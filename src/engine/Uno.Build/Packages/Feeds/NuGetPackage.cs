using System;
using NuGet;
using Uno.IO;
using Uno.Logging;
using System.Threading;

namespace Uno.Build.Packages.Feeds
{
    public class NuGetPackage : LogObject, IPackage
    {
        readonly IPackageRepository _repo;
        readonly NuGet.IPackage _package;

        public string Name => _package.Id;
        public string Source => _repo.Source;

        public string Version
        {
            get
            {
                try
                {
                    return _package.Version.ToString();
                }
                catch (Exception e)
                {
                    Log.Trace(e);
                    return "0.0.0-unknown";
                }
            }
        }

        public NuGetPackage(Log log, IPackageRepository repo, NuGet.IPackage package)
            : base(log)
        {
            _repo = repo;
            _package = package;
        }

        public void Install(string directory)
        {
            int failures = 0;
            const int maxRetries = 5;
            while (true)
            {
                try
                {
                    using (var f = _package.GetStream())
                    {
                        Log.Verbose("Extracting to " + directory.ToRelativePath().Quote());
                        f.ExtractNupkg(directory);
                    }
                    return;
                }
                catch (Exception e)
                {
                    failures++;
                    if (failures == maxRetries)
                        throw;

                    Log.Trace(e);
                    Log.Warning("Failed to install package, retrying: " + e.Message);
                    Thread.Sleep(100);
                }
            }
        }

        public override string ToString()
        {
            return _package.ToString();
        }
    }
}
