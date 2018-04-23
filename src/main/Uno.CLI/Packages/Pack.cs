using System.Collections.Generic;
using Mono.Options;
using Uno.Build.Packages;
using Uno.Build.Targets;
using Uno.ProjectFormat;

namespace Uno.CLI.Packages
{
    class Pack : Command
    {
        public override string Name => "pack";
        public override string Description => "Build project(s) and make Uno package(s).";
        public override bool IsExperimental => true;

        public override void Help()
        {
            WriteUsage("[options] [project-path ...]");

            WriteHead("Available options", 24);
            WriteRow("-o, --out-dir=PATH",    "Specify output directory", true);
            WriteRow("-b, --build-dir=PATH",  "Specify build directory", true);
            WriteRow("-n, --version=STRING",  "Specify package version", true);
            WriteRow("-s, --suffix=STRING",   "Specify package suffix", true);
            WriteRow("-r, --recursive",       "Look for project files recursively");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var recursive = false;
            var builder = new UpkBuilder(Disk, BuildTargets.Package);
            builder.BuildAll(new OptionSet {
                    { "o=|out-dir=|output-dir=",  value => builder.OutputDirectory = value },
                    { "b=|build-dir=",            value => builder.BuildDirectory = value },
                    { "n=|version=",              value => builder.Version = value },
                    { "s=|suffix=",               value => builder.Suffix = value },
                    { "r|recursive",              value => recursive = true }
                }.Parse(args)
                .Paths(".")
                .GetProjectFiles(recursive));
        }
    }
}
