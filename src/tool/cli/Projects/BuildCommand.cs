using System.Collections.Generic;
using System.IO;
using Mono.Options;
using Uno.Build;
using Uno.Build.Targets;
using Uno.Logging;
using Uno.ProjectFormat;

namespace Uno.CLI.Projects
{
    public class BuildCommand : Command
    {
        public override string Name => "build";
        public override string Description => "Build a project for given target.";

        public override void Help()
        {
            WriteUsage("[target] [options] [project-path]");

            WriteHead("Examples", 26);
            WriteRow("uno build android",           "Build Android app, in current directory");
            WriteRow("uno build ios --run",         "Build & run iOS app, in current directory");
            WriteRow("uno build native --debug",    "Build & open Visual C++ or Xcode, if available");

            WriteHead("Common options", 26);
            WriteRow("-c, --configuration=STRING",  "Build configuration [Debug|Release]");
            WriteRow("-t, --target=STRING",         "Build target (see: Available build targets)");
            WriteRow("-d, --debug",                 "Open IDE for debugging after successful build");
            WriteRow("-r, --run",                   "Start the program after successful build");
            WriteRow("-z, --clean",                 "Clean the output directory before building");
            WriteRow("-v, -vv, -vvv",               "Increment verbosity level");

            WriteHead("Additional options", 26);
            WriteRow("-n, --native-args=STRING",    "Arguments to native build command");
            WriteRow("-a, --run-args=STRING",       "Arguments to run command");
            WriteRow("-m, --main=STRING",           "Override application entrypoint");
            WriteRow("-s, --set:NAME=STRING",       "Override build system property");
            WriteRow("-o, --out-dir=PATH",          "Override output directory");
            WriteRow("-b, --build-only",            "Build only; don't run or open debugger");
            WriteRow("-g, --gen-only",              "Generate only; don't compile generated code.");
            WriteRow("-f, --force",                 "Build even if output is up-to-date");
            WriteRow("-l, --libs",                  "Rebuild package library if necessary");
            WriteRow("-p, --print-internals",       "Print a list of build system properties");
            WriteRow("-N, --no-native",             "Disable native build step (faster)");
            WriteRow("-P, --no-parallel",           "Disable multi-threading (slower)");
            WriteRow("-S, --no-strip",              "Disable removal of unused code (slower)");

            WriteHead("Compiler options", 26);
            WriteRow("-D, --define=STRING",         "Add define, to enable a feature");
            WriteRow("-U, --undefine=STRING",       "Remove define, to disable a feature");
            WriteRow("-E, --max-errors=NUMBER",     "Set max error count (0 = disable)");
            WriteRow("-W<0..3>",                    "Set warning level (0 = disable)");

            WriteHead("C++ options", 26);
            WriteRow("-DREFLECTION",                "Enable run-time type reflection");
            WriteRow("-DSTACKTRACE",                "Enable stack traces on Exception");
            WriteRow("-DDEBUG_UNSAFE",              "Enable C++ asserts in unsafe code");
            WriteRow("-DDEBUG_NATIVE",              "Disable C++ optimizations when debugging");

            if (Log.EnableExperimental)
            {
                WriteRow("-DDEBUG_ARC<0..4>",       "Log events from ARC/memory management");
                WriteRow("-DDEBUG_DUMPS",           "Dump GraphViz files to help identify cycles in memory");

                WriteHead("GLSL options", 26);
                WriteRow("-DDUMP_SHADERS",          "Dump shaders to build directory for inspection");
            }

            WriteHead("Available build targets", 19);

            foreach (var c in BuildTargets.Enumerate(Log.EnableExperimental))
                WriteRow("* " + c.Identifier, c.Description);
        }

        public override void Execute(IEnumerable<string> args)
        {
            var buildResult = Build(Parse(args));
             
            // Fix exit code in case of internal or unexpected compiler error.
            ExitCode = Log.ErrorCount == 0 
                ? buildResult.ErrorCount
                : Log.ErrorCount;
        }

        public static BuildArguments Parse(IEnumerable<string> args, Log log = null)
        {
            if (log == null)
                log = Log;

            string targetName = null;
            var options = new BuildOptions();
            var nativeArgs = new List<string>();
            var runArgs = new List<string>();
            var run = false;
            var buildOnly = false;
            var genOnly = false;
            var input = new OptionSet {
                    { "t=|target=",             value => targetName = value },
                    { "c=|configuration=",      value => options.Configuration = value.ParseEnum<BuildConfiguration>("configuration") },
                    { "s=|set=",                value => value.ParseProperty("s|set", options.Settings) },
                    { "p|print-internals",      value => options.PrintInternals = true },
                    { "o=|out-dir=|output-dir=",    value => options.OutputDirectory = value },
                    { "m=|main=|main-class=",   value => options.MainClass = value },
                    { "n=|native-args=",        nativeArgs.Add },
                    { "a=|run-args=",           runArgs.Add },
                    { "P|no-parallel",          value => options.Parallel = false },
                    { "N|q|no-native",          value => options.NativeBuild = false },
                    { "S|e|no-strip",           value => options.Strip = false },
                    { "E=|max-errors=",         value => Log.MaxErrorCount = value.ParseInt("E") },
                    { "W=",                     value => options.WarningLevel = value.ParseInt("W") },
                    { "D=|define=",             options.Defines.Add },
                    { "U=|undefine=",           options.Undefines.Add },
                    { "release",                value => options.Configuration = BuildConfiguration.Release },
                    { "test",                   value => options.Test = true },
                    { "z|clean",                value => options.Clean = true },
                    { "d|debug",                value => runArgs.Add("debug") },
                    { "r|run",                  value => run = true },
                    { "b|build-only",           value => buildOnly = true },
                    { "g|gen-only",             value => genOnly = true },
                    { "l|libs",                 value => options.UpdateLibrary = true },
                    { "f|force",                value => options.Force = true },
                    { "cd=",                    value => Directory.SetCurrentDirectory(value.ParseString("cd")) },
                    { "v",                      value => log.Level++ },
                    { "x",                      value => Log.EnableExperimental = true }
                }.Parse(args);

            var target = BuildTargets.Get(targetName, input);
            var project = input.Path(".").GetProjectFile();
            options.NativeArguments = string.Join(" ", nativeArgs);
            options.RunArguments = string.Join(" ", runArgs);

            if (runArgs.Count > 0 && runArgs[0] == "debug")
            {
                options.NativeBuild = false; // disable native build
                options.Defines.Add("DEBUG_NATIVE"); // disable native optimizations (debug build)
            }

            if (buildOnly || genOnly)
            {
                if (genOnly)
                    options.NativeBuild = false;

                runArgs.Clear();
                run = false;
            }

            return new BuildArguments
            {
                Log = log,
                Options = options,
                Target = target,
                ProjectFile = project,
                Run = run || runArgs.Count > 0,
            };
        }

        public static BuildResult Build(BuildArguments args)
        {
            args.Log.ProductHeader();
            var result = new ProjectBuilder(args.Log, args.Target, args.Options)
                        .Build(args.ProjectFile);

            if (args.Run && result.ErrorCount == 0)
                result.Run();

            return result;
        }
    }
}
