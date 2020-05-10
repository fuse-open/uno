using System;
using System.Collections.Generic;
using Mono.Options;
using Uno.Build;
using Uno.Build.Targets;
using Uno.ProjectFormat;

namespace Uno.CLI.Projects
{
    public class NoBuild : Command
    {
        public override string Name => "no-build";
        public override string Description => "Invoke generated build steps without triggering a build.";
        public override bool IsExperimental => true;

        public override void Help()
        {
            WriteUsage("[target] [options] [project-path]");

            WriteHead("Common options", 26);
            WriteRow("-c, --configuration=STRING",  "Build configuration [Debug|Release]");
            WriteRow("-o, --out-dir=PATH",          "Specify output directory", true);
            WriteRow("-b, --build",                 "Execute native build command");
            WriteRow("-d, --debug",                 "Open IDE for debugging");
            WriteRow("-r, --run",                   "Start the program");
            WriteRow("-v, -vv, ...",                "Increment verbosity level");

            WriteHead("Additional options", 26);
            WriteRow("-n, --native-args=ARGS",      "Arguments to native build command");
            WriteRow("-a, --run-args=ARGS",         "Arguments to run command");
            WriteRow("-t, --target=STRING",         "Build target (see: uno build --help)");
        }

        public override void Execute(IEnumerable<string> args)
        {
            string targetName = null;
            string outputDir = null;
            var configuration = BuildConfiguration.Debug;
            var nativeArgs = new List<string>();
            var runArgs = new List<string>();
            var build = false;
            var run = false;
            var input = new OptionSet
                {
                    { "t=|target=",             value => targetName = value },
                    { "c=|configuration=",      value => configuration = value.ParseEnum<BuildConfiguration>("configuration") },
                    { "o=|out-dir|output-dir=", value => outputDir = value },
                    { "n=|native-args=",        nativeArgs.Add },
                    { "a=|run-args=",           runArgs.Add },
                    { "b|build",                value => build = true },
                    { "d|debug",                value => runArgs.Add("debug") },
                    { "r|run",                  value => run = true },
                }
                .Parse(args);

            var target = BuildTargets.Get(targetName, input);
            var project = Project.Load(input.Path(".").GetProjectFile());

            if (string.IsNullOrEmpty(outputDir))
                outputDir = project.OutputDirectory
                    .Replace("@(Target)", target.Identifier)
                    .Replace("@(Configuration)", configuration.ToString());

            var file = new BuildFile(outputDir);

            if (!file.Exists)
                throw new InvalidOperationException("Target was not built -- run 'uno build' first");

            if (build || nativeArgs.Count > 0)
            {
                if (!target.CanBuild(file))
                    // If the build script is missing it's probably not needed
                    Log.Warning("A build script was not found in " + outputDir.Quote());
                else if (!target.Build(Shell, file, string.Join(" ", nativeArgs)))
                {
                    ExitCode = 1;
                    return;
                }
            }

            if (run || runArgs.Count > 0)
                if (!target.CanRun(file) ||
                    !target.Run(Shell, file, string.Join(" ", runArgs)).Result)
                    ExitCode = 1;
        }
    }
}
