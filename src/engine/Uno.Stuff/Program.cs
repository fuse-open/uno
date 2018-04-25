using System;
using System.Collections.Generic;
using System.Linq;
using Stuff.Commands;
using Stuff.Core;
using Stuff.Options;

namespace Stuff
{
    public class Program : Command
    {
        readonly Command[] _commands =
        {
            new Install(),
            new Clean(),
            new Pack(),
            new Push(),
            new Commands.GC(),
            new Parse(),
            new Sln(),
            new Symlink()
        };

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("COMMAND [args]", "--version");
            WriteHead("Available commands", 11);

            foreach (var cmd in _commands.Where(x => x.Description != null))
                WriteRow(cmd.Name, cmd.Description);

            WriteLine("\nType \"" + Root + " COMMAND --help\" to get instructions for a command");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var showHelp = false;
            var input = new OptionSet {
                    {"h|?|help",   x => showHelp = x != null},
                    {"v|verbose",  x => Log.EnableVerbose = x != null},
                }.Parse(args);

            var cmdName = input.FirstOrDefault();

            switch (cmdName)
            {
                case null:
                    // Implicit help on root command
                    Help(input);
                    return;
                case "upload":
                    // Deprecated command
                    Log.Warning("'upload' is deprecated -- please use 'push' from now on.");
                    cmdName = "push";
                    break;
                case "--cmd-ref":
                    WritePage();
                    foreach (var c in _commands)
                        WritePage(c.Name, "--help");
                    return;
                case "--version":
                    WriteLine("Nazareth Stuff System version " + ExeVersion);
                    WriteLine("Copyright (C) 2015-2017 Fusetools");
                    WriteEnvironment();
                    return;
            }

            var cmd = _commands.FirstOrDefault(x => x.Name == cmdName);

            if (cmd == null)
                throw new ArgumentException(cmdName.Quote() + " is not a valid command -- type \"" + Root + " --help\" to list available commands");

            if (showHelp)
                cmd.Help(input.Skip(1));
            else
                cmd.Execute(input.Skip(1));
        }

        void WritePage(params string[] args)
        {
            Log.WriteLine(ConsoleColor.Blue, "##### $ " + Root + " " +
                string.Join(" ", args).Replace(" --help", ""));
            Log.WriteLine(ConsoleColor.Blue, "```");
            Execute(args);
            Log.WriteLine(ConsoleColor.Blue, "```");
            Log.WriteLine(ConsoleColor.Blue, "");
        }
    }
}