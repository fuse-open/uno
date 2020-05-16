using System;
using System.Collections.Generic;
using System.IO;
using Mono.Options;
using Uno.IO;
using Uno.ProjectFormat;

namespace Uno.CLI.Projects
{
    class Create : Command
    {
        public override string Name => "create";
        public override string Description => "Create a new project file.";

        public override void Help()
        {
            WriteUsage("[options] [file|directory]");

            WriteHead("Available options");
            WriteRow("-c, --class=NAME",    "Initialize project with an empty class with this name", true);
            WriteRow("-n, --name=NAME",     "Specify project file name", true);
            WriteRow("-d, --defaults",      "Add default settings");
            WriteRow("-e, --empty",         "Create empty project without packages or items");
            WriteRow("-f, --force",         "Overwrite any existing project without warning");
            WriteRow("    --flatten",       "Flatten items to explicit list of files");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var force = false;
            string projectName = null;
            string className = null;
            var empty = false;
            var flatten = false;
            var defaults = false;

            var input = new OptionSet {
                    { "c|class=",   value => className = value },
                    { "n|name=",    value => projectName = value },
                    { "d|defaults", value => defaults = true },
                    { "e|empty",    value => empty = true },
                    { "f|force",    value => force = true },
                    { "flatten",    value => flatten = true }
                }.Parse(args);

            if (input.Count == 0)
                input.Add(".");
            else
                input.CheckArguments();

            if (input.Count != 1 || string.IsNullOrWhiteSpace(input[0]))
                throw new ArgumentException("Expected one project [file|directory]");

            var fullName = input[0].ToFullPath();

            if (projectName == null)
                projectName = Path.GetFileName(fullName);

            if (!fullName.ToUpperInvariant().EndsWith(".UNOPROJ"))
                fullName = Path.Combine(fullName, projectName + ".unoproj");

            if (Directory.Exists(fullName))
                throw new Exception(fullName.Quote() + " is a directory");

            if (!force && File.Exists(fullName))
                throw new Exception(fullName.Quote() + " already exists");

            var dirName = Path.GetDirectoryName(fullName);
            Disk.CreateDirectory(dirName);

            WriteLine("Creating project " + fullName.Quote());

            var project = new Project(fullName);

            if (defaults)
                project.AddDefaults();

            if (!empty)
            {
                project.MutableIncludeItems.Add("*");

                foreach (var e in project.Config.GetStringArray("Packages.Default") ?? new string[0])
                    project.MutablePackageReferences.Add(e);
            }

            if (className != null)
            {
                var fileName = Path.Combine(project.RootDirectory, className + ".uno");
                WriteEmptyClass(className, project.Name, fileName);

                if (empty)
                    project.MutableIncludeItems.Add(className + ".uno");
            }

            if (flatten)
                project.FlattenItems(Log);

            project.Save();
        }

        private static void WriteEmptyClass(string className, string projName, string fullName)
        {
            File.WriteAllText(fullName,
                string.Format(AppTemplate, projName.ToIdentifier(), className.ToIdentifier()));
        }

        private const string AppTemplate =
@"using Uno;

namespace {0}
{{
    public class {1} : Application
    {{
    }}
}}
";
    }
}
