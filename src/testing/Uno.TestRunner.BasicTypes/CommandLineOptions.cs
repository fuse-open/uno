using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Mono.Options;
using Uno.Build;
using Uno.Build.Targets;

namespace Uno.TestRunner.BasicTypes
{
    public class CommandLineOptions
    {
        public List<string> Paths;
        public string LogFileName;
        public BuildTarget Target;
        public bool Verbose;
        public string Filter;
        public bool Trace;
        public bool OnlyBuild;
        public bool NoUninstall;
        public bool Library;
        public string OutputDirectory;
        public readonly List<string> Defines = new List<string>();
        public readonly List<string> Undefines = new List<string>();

        public static CommandLineOptions From(string[] args)
        {
            var help = false;
            var verbose = false;
            var quiet = false;

            var commandOptions = new CommandLineOptions();

            string targetName = null;
            var p = new OptionSet
            {
                { "h|?|help", "Show help", v => help = v != null },
                { "r|reporter=", "Reporter type (not used)",  v => Console.Error.WriteLine("WARNING: --reporter is deprecated and has no effect.") },
                { "l|logfile=", "Write output to this file instead of stdout", v => commandOptions.LogFileName = v },
                { "t|target=", "Build target. Supported: android, dotnet and native", v => targetName = v },
                { "v|verbose", "Verbose, always prints output from compiler and debug_log", v => verbose = v != null },
                { "q|quiet", "Quiet, only prints output from compiler and debug_log in case of errors.", v => quiet = v != null },
                { "f|filter=", "Only run tests matching this string", v => commandOptions.Filter = Regex.Escape(v) },
                { "e|regex-filter=", "Only run tests matching this regular expression", v => commandOptions.Filter = v },
                { "o|timeout=", "Timeout for individual tests (in seconds)", (int v) => Console.Error.WriteLine("WARNING: --timeout is deprecated and has no effect.") },
                { "startup-timeout=", "Timeout for connection from uno process (in seconds)", (int v) => Console.Error.WriteLine("WARNING: --startup-timeout is deprecated and has no effect.") },
                { "trace", "Print trace information from unotest", v => { commandOptions.Trace = v != null; } },
                { "only-build", "Don't run compiled program.",  v => commandOptions.OnlyBuild = v != null },
                { "allow-debugger", "Don't run compiled program, allow user to start it from a debugger.",  v => Console.Error.WriteLine("WARNING: --allow-debugger is deprecated and has no effect.") },
                { "d|debug", "Open IDE for debugging tests.",  v => Console.Error.WriteLine("WARNING: --debug is deprecated and has no effect.") },
                { "run-local", "Run the test directly (not used)",  v => Console.Error.WriteLine("WARNING: --run-local is deprecated and has no effect.") },
                { "no-uninstall", "Don't uninstall tests after running on device", v => commandOptions.NoUninstall = v != null },
                { "D=|define=", "Add define, to enable a feature", commandOptions.Defines.Add },
                { "U=|undefine=", "Remove define, to disable a feature", commandOptions.Undefines.Add },
                { "output-dir=", "Override output directory", v => commandOptions.OutputDirectory = v },
                { "libs", "Rebuild package library if necessary", v => commandOptions.Library = true },
            };

            try
            {
                var extra = p.Parse(args);
                commandOptions.Target = BuildTargets.Get(targetName, extra);
                commandOptions.Paths = extra;
                commandOptions.Verbose = verbose;

                if (verbose && quiet)
                    throw new ArgumentException("Cannot specify both -q and -v");
            }
            catch (OptionException e)
            {
                Console.WriteLine(e);
                PrintHelp(p);
                return null;
            }

            if (help)
            {
                PrintHelp(p);
                return null;
            }

            return commandOptions;
        }

        private static void PrintHelp(OptionSet p)
        {
            Console.WriteLine("Usage: uno test [target] [options] [paths-to-search]");
            Console.WriteLine();
            Console.WriteLine("[paths-to-search] is a list of paths to unoprojs to run tests from, and/or");
            Console.WriteLine("directories in which to search for test projects.");
            Console.WriteLine("When a directory is given, uno test searches recursively in that directory");
            Console.WriteLine("for projects named '*Test.unoproj'");
            Console.WriteLine();
            Console.WriteLine("Available options:");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine(@"  uno test");
            Console.WriteLine(@"  uno test path/projects");
            Console.WriteLine(@"  uno test path/projects/FooTest.unoproj path/projects/BarTest.unoproj");
            Console.WriteLine(@"  uno test path/projects path/other-projects/FooTest.unoproj");
            Console.WriteLine(@"  uno test native -v path/projects");
        }
    }
}
