using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Options;
using Uno.Build.Packages;
using Uno.IO;

namespace Uno.CLI.Packages
{
    class Uninstall : Command
    {
        public override string Name => "uninstall";
        public override string Description => "Uninstall Uno package(s) from the local cache.";
        public override bool IsExperimental => true;

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("<package|wildcard ...> [version]");

            WriteHead("Uninstall options");
            WriteRow("-n, --version=STRING",  "Uninstall a specific version of <package>");
            WriteRow("-f, --force",           "Uninstall all matching packages without warning");
        }

        public override void Execute(IEnumerable<string> args)
        {
            string version = null;
            IPackageFeed source = null;
            var force = false;
            var input = new OptionSet {
                    { "n=|version=",            value => version = value },
                    { "f|force",                value => force = true }
                }.Parse(args);

            if (input.Count == 0)
                throw new ArgumentException("No packages specified");

            input.CheckArguments();
            Feed.FilterPackageArguments(input, ref version, ref source);

            var cache = new PackageCache();
            var versions = new List<DirectoryInfo>();

            foreach (var package in input)
                versions.AddRange(cache.EnumerateVersions(package, version));

            if (versions.Count == 0)
            {
                if (force)
                    return;
                throw new ArgumentException("No packages found");
            }

            if (!force && versions.Count > 1)
            {
                foreach (var dir in versions)
                    WriteLine(dir.FullName.ToRelativePath());

                if (!Confirm("The search returned more than one package -- delete all of them?"))
                    return;
            }

            foreach (var dir in versions)
                Disk.DeleteDirectory(dir.FullName, true);

            // Delete any remaining empty directories
            foreach (var package in input)
                foreach (var dir in cache.EnumeratePackages(package))
                    if (dir.EnumerateFileSystemInfos().FirstOrDefault() == null)
                        Disk.DeleteDirectory(dir.FullName, true);
        }
    }
}