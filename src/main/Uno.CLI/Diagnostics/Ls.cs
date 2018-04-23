using System.Collections.Generic;
using System.Linq;
using Mono.Options;
using Uno.IO;
using Uno.ProjectFormat;

namespace Uno.CLI.Diagnostics
{
    class Ls : Command
    {
        public override string Name => "ls";
        public override string Description => "Print project items found to STDOUT.";

        public override void Help()
        {
            WriteUsage("[options] [project-path ...]");

            WriteHead("Available options");
            WriteRow("-r, --recursive",       "Look for project files recursively");
            WriteRow("-p, --projects",        "List project references instead of files");
            WriteRow("-F, --no-files",        "Don't list files included in project(s)");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var recursive = false;
            var view = View.Files;

            foreach (var f in new OptionSet {
                        {"r|recursive",  value => recursive = true},
                        {"p|projects",   value => view = View.Projects},
                        {"F|no-files",   value => view = 0}
                    }.Parse(args)
                    .Paths(".")
                    .GetProjectFiles(recursive))
            {
                WriteHead(f.ToRelativePath(), 60);
                switch (view)
                {
                    case View.Files:
                        foreach (var e in Project.Load(f)
                                .GetFlattenedItems(Log, true))
                            WriteRow(e.Value.UnixToNative(),
                                string.Join(", ", new[] {
                                        e.Type.ToString(),
                                        e.Condition?.ToUpperInvariant()
                                    }.Where(value => true)));
                        break;
                    case View.Projects:
                        foreach (var e in Project.Load(f)
                                .GetFlattenedProjects(Log, true))
                            WriteRow(e.ProjectPath.UnixToNative());
                        break;
                }
            }
        }

        enum View
        {
            Files = 1,
            Projects
        }
    }
}
