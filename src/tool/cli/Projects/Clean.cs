using System.Collections.Generic;
using Mono.Options;
using Uno.Build;
using Uno.Build.Targets;
using Uno.ProjectFormat;

namespace Uno.CLI.Projects
{
    class Clean : Command
    {
        public override string Name => "clean";
        public override string Description => "Delete generated build and cache directories in project(s).";

        public override void Help()
        {
            WriteUsage("[target] [options] [project-path ...]");

            WriteHead("Examples", 28);
            WriteRow("uno clean",                    "Clean all build-files (in current directory)");
            WriteRow("uno clean android -c Release", "Clean only Android files (Release configuration)");

            WriteHead("Available options", 26);
            WriteRow("-t, --target=STRING",         "Build target (see: Available build targets)");
            WriteRow("-c, --configuration=STRING",  "Build configuration [Debug|Release]");
            WriteRow("-r, --recursive",             "Look for project files recursively");

            WriteHead("Available build targets", 19);

            foreach (var c in BuildTargets.Enumerate(Log.EnableExperimental))
                WriteRow("* " + c.Identifier, c.Description);
        }

        public override void Execute(IEnumerable<string> args)
        {
            var recursive = false;
            string targetName = null;
            BuildConfiguration configuration = 0;

            var input = new OptionSet {
                    {"t=|target=",             value => targetName = value },
                    {"c=|configuration=",      value => configuration = value.ParseEnum<BuildConfiguration>("configuration") },
                    {"r|recursive",            value => recursive = true}
                }.Parse(args);
            var target = BuildTargets.Get(targetName, input, canReturnNull:true);
            var files = input.Paths(".").GetProjectFiles(recursive);

            new ProjectCleaner(Log, target, configuration)
                .Clean(files);
        }
    }
}
