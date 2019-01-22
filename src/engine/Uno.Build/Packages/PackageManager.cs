using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uno.Build.Packages.Feeds;
using Uno.Build.Stuff;
using Uno.Configuration;
using Uno.Configuration.Format;
using Uno.IO;
using Uno.Logging;
using Uno.ProjectFormat;
using Log = Uno.Logging.Log;

namespace Uno.Build.Packages
{
    public class PackageManager : LogObject, IPackageFeed
    {
        public static string CacheDir = UnoConfig.Current.GetString("Packages.CacheDirectory") ?? Path.Combine(Path.GetTempPath(), ".uno");
        public static string DefaultInstallDirectory => UnoConfig.Current.GetFullPath("Packages.InstallDirectory");
        public static string[] FeedSources => UnoConfig.Current.GetStringArray("Packages.Feeds");

        public static readonly IPackageFeed CacheFeed = new CacheFeed();

        public IPackageFeed GetSource(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            return new NuGetFeed(Log, source);
        }

        string _installDir;
        public string InstallDirectory
        {
            get { return _installDir ?? DefaultInstallDirectory; }
            set { _installDir = value; }
        }

        public int InstallCount { get; private set; }
        public IPackageFeed Source;
        public bool Force;

        public PackageManager(Log log)
            : base(log)
        {
        }

        public void InstallFile(string filename)
        {
            switch ((Path.GetExtension(filename) ?? "").ToUpperInvariant())
            {
                case ".NUPKG":
                case ".UPK":
                    Install(new UpkFile(filename));
                    break;
                case ".PACKAGES":
                    InstallAll(StuffObject.Load(filename));
                    break;
                case ".UNOPROJ":
                    InstallAll(Project.Load(filename));
                    break;
                default:
                    throw new InvalidOperationException("Unsupported file: " + filename.ToRelativePath());
            }
        }

        public void InstallAll(IDictionary<string, object> packages)
        {
            var tasks = new List<Task>();

            foreach (var package in packages.Keys)
                foreach (var version in packages.GetArray(package))
                    tasks.Add(Task.Factory.StartNew(() => Install(package, version)));

            Task.WaitAll(tasks.ToArray());
            InstallCount++; // Avoid error later
        }

        public void InstallAll(Project project)
        {
            using (var pc = new PackageCache(Log, project.Config, this))
                pc.GetPackage(project);

            InstallCount++; // Avoid error later
        }

        public PackageFile Install(string name, string version = null)
        {
            if (!Force)
            {
                var upk = GetInstalled(name, version);

                if (upk != null)
                {
                    try
                    {
                        var file = PackageFile.Load(upk.Source);
                        CheckInstalled(file);
                        return file;
                    }
                    catch (Exception e)
                    {
                        Log.Trace(e);
                    }
                }
            }

            var cached = Path.Combine(CacheDir, name + "." + version + ".nupkg");
            if (File.Exists(cached))
            {
                try
                {
                    return Install(new UpkFile(cached, name, version));
                }
                catch (Exception e)
                {
                    Log.Trace(e);
                }
            }

            return Install(FindPackage(name, version));
        }

        public PackageFile Install(IPackage upk)
        {
            var dst = Path.Combine(InstallDirectory, upk.Name, upk.Version);
            Log.WriteLine("Installing " + upk.Name + " " + upk.Version, ConsoleColor.Blue);

            using (new FileLock(dst))
            {
                if (Force || !PackageFile.Exists(dst))
                    upk.Install(dst);

                var file = PackageFile.Load(dst);
                CheckInstalled(file);
                return file;
            }
        }

        void CheckInstalled(PackageFile file)
        {
            // Update cache if necessary
            new PackageDoctor(Log).Repair(file);
            InstallCount++;
        }

        public bool IsInstalled(string name, string version = null)
        {
            return GetInstalled(name, version) != null;
        }

        public IPackage GetInstalled(string name, string version = null)
        {
            return CacheFeed
                .FindPackages(new[] {name}, version)
                .FirstOrDefault();
        }

        public IPackage FindPackage(string name, string version = null)
        {
            return FindPackages(name, version)[0];
        }

        public IPackage[] FindPackages(string name, string version = null)
        {
            var packages = (Source ?? this)
                .FindPackages(new[] {name}, version)
                .ToArray();

            if (packages.Length == 0)
                throw new InvalidOperationException("Package not found: " + name + " (" + version + ")");

            return packages;
        }

        IEnumerable<IPackage> IPackageFeed.FindPackages(IReadOnlyList<string> names, string version)
        {
            foreach (var feed in FeedSources
                    .Select(GetSource)
                    .ToArray())
                foreach (var package in feed
                        .FindPackages(names, version))
                    yield return package;
        }
    }
}
