using System.Collections.Generic;
using Mono.Options;
using Uno.Build;
using Uno.ProjectFormat;

namespace Uno.CLI.Projects
{
    class Clean : Command
    {
        public override string Name => "clean";
        public override string Description => "Delete generated build and cache directories in project(s).";

        public override void Help()
        {
            WriteUsage("[options] [project-path ...]");

            WriteHead("Available options");
            WriteRow("-r, --recursive",       "Look for project files recursively");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var recursive = false;
            var files = new OptionSet {
                    {"r|recursive", value => recursive = true}
                }.Parse(args)
                .Paths(".")
                .GetProjectFiles(recursive);

            new ProjectCleaner(Log)
                .Clean(files);
        }
    }
}
