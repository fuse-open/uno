using System;
using System.Collections.Generic;
using System.IO;
using Mono.Options;
using Uno.Build.Packages;
using Uno.ProjectFormat;

namespace Uno.CLI.Packages
{
    class Install : Command
    {
        public override string Name => "install";
        public override string Description => "Install Uno package(s) to the local cache.";
        public override bool IsExperimental => true;

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("<package ...> [version] [source]",
                       "<project-file|directory>",
                       "<package-list-file>",
                       "<upk-file ...>");

            WriteHead("Install options");
            WriteRow("-d, --directory=PATH",      "Specify a custom install directory", true);
            WriteRow("-n, --version=STRING",      "Install a specific version of <package>");
            WriteRow("-s, --source=URL",          "Install <package> from a specific source");
            WriteRow("-f, --force",               "Install a package even if already installed");
        }

        public override void Execute(IEnumerable<string> args)
        {
            string version = null;
            var pm = new PackageManager(Log);
            var input = new List<string>(
                new OptionSet {
                        { "d=|directory=",          value => pm.InstallDirectory = value },
                        { "n=|version=",            value => version = value },
                        { "s=|source=",             value => pm.Source = pm.GetSource(value) },
                        { "f|force",                value => pm.Force = true }
                    }.Parse(args)
                    .Paths("."));

            Feed.FilterPackageArguments(input, ref version, ref pm.Source);
            Log.StartAnimation();

            for (int i = 0; i < input.Count; i++)
            {
                var arg = input[i];

                // Look for .UPK files when wildcard is given
                if (arg.IndexOf('*') != -1)
                {
                    foreach (var f in new[] {arg}.Files("*.upk"))
                        pm.InstallFile(f);

                    continue;
                }

                if (Directory.Exists(arg))
                    arg = arg.GetProjectFile();

                if (File.Exists(arg))
                    pm.InstallFile(arg);
                else
                    pm.Install(arg, version);
            }

            if (!pm.Force && pm.InstallCount == 0)
                throw new ArgumentException("No packages installed");
        }
    }
}
