using System.Collections.Generic;
using Mono.Options;
using Uno.IO;
using Uno.ProjectFormat;

namespace Uno.CLI.Projects
{
    class Update : Command
    {
        public override string Name => "update";
        public override string Description => "Update packages and project(s).";

        public override void Help()
        {
            WriteUsage("[options] [project-path ...]");

            WriteHead("Example", 24);
            WriteRow("uno update -pr --files",  "Update projects recursively, adding new files");

            WriteHead("Common options", 22);
            WriteRow("    (default)",           "Install/update packages");
            WriteRow("-p, --project",           "Update project file(s)");
            WriteRow("-r, --recursive",         "Look for project files recursively");

            WriteHead("Project options", 22);
            WriteRow("-d, --defaults",          "Add default values");
            WriteRow("-s, --strip",             "Remove properties with default values");
            WriteRow("-c, --clear",             "Remove all include items");
            WriteRow("-e, --exclude",           "Remove include items that are exluded");
            WriteRow("-f, --files",             "Scan project directory for new files to add/remove");
            WriteRow("-g, --glob=PATTERN",      "Scan project directory for matching files to add/remove");
            WriteRow("    --flatten",           "Flatten items to explicit list of files");
            WriteRow("    --dry-run",           "Don't save updated project(s)");
        }

        public override void Execute(IEnumerable<string> args)
        {
            bool? defaultOrStrip = null;
            var excludeItems = false;
            var flattenItems = false;
            var globPatterns = new List<string>();
            var clearItems = false;
            var dryRun = false;
            var recursive = false;
            var updateProject = false;
            var files = new OptionSet {
                    { "d|defaults",     value => defaultOrStrip = true },
                    { "s|strip",        value => defaultOrStrip = false },
                    { "c|clear",        value => clearItems = true },
                    { "e|exclude",      value => excludeItems = true },
                    { "p|project",      value => updateProject = true },
                    { "r|recursive",    value => recursive = true },
                    { "g=|glob=",       globPatterns.Add },
                    { "f|files|add-files", value => globPatterns.Add("**") },
                    { "flatten",        value => flattenItems = true },
                    { "dry-run",        value => dryRun = true }
                }.Parse(args)
                .Paths(".")
                .GetProjectFiles(recursive);

            if (!updateProject)
                updateProject = defaultOrStrip != null ||
                                globPatterns.Count > 0 ||
                                excludeItems ||
                                flattenItems ||
                                clearItems;

            foreach (var f in files)
            {
                Log.Verbose("Updating " + f.ToRelativePath());
                var project = Project.Load(f);

                if (updateProject)
                {
                    if (defaultOrStrip != null)
                    {
                        if (defaultOrStrip.Value)
                            project.AddDefaults();
                        else
                            project.RemoveDefaults();
                    }

                    if (project.InternalsVisibleTo.Count > 0)
                        project.MutableInternalsVisibleTo.Sort();
                    if (project.PackageReferences.Count > 0)
                        project.MutablePackageReferences.Sort();
                    if (project.ProjectReferences.Count > 0)
                        project.MutableProjectReferences.Sort();

                    if (clearItems)
                        project.MutableIncludeItems.Clear();

                    if (flattenItems)
                        project.FlattenItems(Log);

                    if (globPatterns.Count > 0)
                        project.GlobItems(Log, globPatterns, excludeItems);

                    if (!dryRun)
                        project.Save();
                }
            }

            // Return 0 even if Log.HasErrors
            ExitCode = 0;
        }
    }
}
