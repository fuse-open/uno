using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Mono.Options;
using Uno.Build.Packages;
using Uno.Diagnostics;

namespace Uno.CLI
{
    public class Program : Command
    {
        readonly Command[] _commands =
        {
            new Projects.Create(),
            new Projects.Update(),
            new Projects.BuildCommand(),
            new Projects.NoBuild(),
            new Projects.Clean(),
            new Projects.Test(),
            new Packages.Doctor(),
            new Diagnostics.Config(),
            new Diagnostics.Ls(),
            new Diagnostics.Lint(),
            new Android.Adb(),
            new Android.LaunchApk(),
            new System.Open(),
        };

        public override void Help()
        {
            WriteUsage("COMMAND [args ...]", "COMMAND --help", "--version");

            WriteHead("Global options");
            WriteRow("-h, -?, --help",      "Get help");
            WriteRow("-v, -vv, -vvv",       "Increment verbosity level");
            WriteRow("-x, --experimental",  "Enable experimental features");
            WriteRow("    --(no-)anim",     "Enable or disable terminal animations");
            WriteRow("    --trace",         "Print exception stack traces where available");

            WriteHead("Available commands", 13);

            foreach (var cmd in _commands.Where(value => !value.IsExperimental))
                WriteRow(cmd.Name, (cmd.Description ?? "(no description)").TrimEnd('.'));

            WriteHead("Experimental commands", 13);

            foreach (var cmd in _commands.Where(value => value.IsExperimental))
                WriteRow(cmd.Name, (cmd.Description ?? "(no description)").TrimEnd('.'));

            WriteHead("Environment variables", 16);
            WriteRow("LOG_LEVEL=<0..3>",  "Set verbosity level (default = 0)");
            WriteRow("LOG_TRACE=1",       "Print exception stack traces where available");
            WriteRow("DEBUG_GL=1",        "Enable the OpenGL debug layer in .NET builds");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var showHelp = false;
            var input = new OptionSet {
                    { "h|?|help", value => showHelp = true },
                    { "x|experimental", value => Log.EnableExperimental = true },
                    { "anim", value => Log.EnableAnimation = true },
                    { "no-anim", value => Log.EnableAnimation = false },
                    { "trace", value => Log.EnableTrace = true },
                    { "v", value => Log.Level++ }
                }.Parse(args);

            // Implicit help when no input
            if (input.Count == 0)
            {
                Help(input);
                return;
            }

            var cmdName = input.First();
            var cmd = _commands.FirstOrDefault(value => value.Name == cmdName);

            // Fuzzy COMMAND finder
            if (cmd == null && !string.IsNullOrEmpty(cmdName) && cmdName[0] != '-')
            {
                foreach (var c in _commands)
                {
                    if ((!c.IsExperimental || Log.EnableExperimental) &&
                        c.Name.StartsWith(cmdName, StringComparison.InvariantCulture))
                    {
                        if (cmd != null)
                        {
                            cmd = null;
                            break;
                        }

                        cmd = c;
                    }
                }
            }

            if (cmd == null)
            {
                var showVersion = false;
                var showCmdRef = false;
                new OptionSet {
                        { "version", value => showVersion = true },
                        { "cmd-ref", value => showCmdRef = true }
                    }.Parse(input);

                if (showVersion)
                {
                    WriteProductInfo();
                    return;
                }

                if (showCmdRef)
                {
                    WritePage();
                    foreach (var e in _commands.Where(value => value.Description != null))
                        WritePage(e.Name, "--help");
                    return;
                }

                var thing = cmdName.StartsWith('-')
                    ? "option"
                    : "command";
                throw new ArgumentException(cmdName.Quote() + $" is not a valid {thing} -- see \"uno --help\" for a list of {thing}s");
            }

            if (showHelp)
                cmd.Help(input.Skip(1));
            else
                cmd.Execute(input.Skip(1));

            ExitCode = cmd.ExitCode ?? Log.ErrorCount;
        }

        public static int Main(params string[] args)
        {
            try
            {
                var p = new Program();
                p.Execute(args);
                return p.ExitCode ?? 0;
            }
            catch (TargetInvocationException e)
            {
                Log.Trace(e);
                Log.Skip();
                Log.WriteErrorLine("ERROR: " + e.InnerException.Message.Trailing('.') + " (pass --trace for stack trace)");
                return 1;
            }
            catch (Exception e)
            {
                Log.Trace(e);
                Log.Skip();
                Log.WriteErrorLine("ERROR: " + e.Message.Trailing('.') + " (pass --trace for stack trace)");
                return 1;
            }
            finally
            {
                Shell.CleanUp();
                Log.Dispose();
            }
        }

        static void WritePage(params string[] args)
        {
            Log.WriteLine("## uno " + string.Join(" ", args).Replace(" --help", ""), ConsoleColor.Blue);
            Log.WriteLine();
            Log.WriteLine("```", ConsoleColor.Blue);
            new Program().Execute(args);
            Log.WriteLine("```", ConsoleColor.Blue);
            Log.WriteLine();
        }

        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        static Program()
        {
            try
            {
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            }
            catch
            {
                //Not supported until Mono 3.2.7
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }
    }
}
