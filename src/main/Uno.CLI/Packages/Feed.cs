using System;
using System.Collections.Generic;
using Mono.Options;
using Uno.Build.Packages;

namespace Uno.CLI.Packages
{
    class Feed : Command
    {
        public override string Name => "feed";
        public override string Description => "Maintain Uno package feeds.";
        public override bool IsExperimental => true;

        public override void Help()
        {
            WriteUsage("[package ...] [version] [source]",
                       "[package ...] [version] --installed");

            WriteHead("Example", 28);
            WriteRow("uno feed UnoCore 1.0.0",    "Look up 'UnoCore' version 1.0.0");

            WriteHead("Filter options", 24);
            WriteRow("    (default)",         "Show packages from all available feeds");
            WriteRow("-i, --installed",       "Show installed or built packages");
            WriteRow("-s, --source=URL",      "Show packages from a remote source");
            WriteRow("-n, --version=STRING",  "Show only packages of this version");
        }

        public override void Execute(IEnumerable<string> args)
        {
            IPackageFeed source = null;
            string version = null;
            var input = new OptionSet {
                    { "i|installed",         value => source = PackageManager.CacheFeed },
                    { "s=|source=",          value => source = new PackageManager(Log).GetSource(value) },
                    { "n=|version=",         value => version = value }
                }.Parse(args);

            if (source == null)
                source = new PackageManager(Log);

            input.CheckArguments();
            FilterPackageArguments(input, ref version, ref source);
            Log.StartAnimation();

            var first = true;
            foreach (var p in source.FindPackages(input, version))
            {
                if (first)
                {
                    WriteHead("Package".PadRight(32) + " " + "Version".PadRight(32) + " Source");
                    first = false;
                }

                WriteLine(p.Name.PadRight(32) + " " + p.Version.PadRight(32) + " " + p.Source);
            }

            if (first)
                throw new ArgumentException("No packages found");
        }

        internal static void FilterPackageArguments(List<string> args, ref string version, ref IPackageFeed source)
        {
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                if (arg.Length > 0 && char.IsNumber(arg[0]))
                {
                    if (version != null)
                        Log.Warning("Overwriting old 'version'");
                    version = arg;
                    args.RemoveAt(i--);
                }
                else if (arg.Contains("://"))
                {
                    if (source != null)
                        Log.Warning("Overwriting old 'source'");

                    source = new PackageManager(Log).GetSource(arg);
                    args.RemoveAt(i--);
                }
            }
        }
    }
}
