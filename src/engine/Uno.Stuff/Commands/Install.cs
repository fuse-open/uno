using System;
using System.Collections.Generic;
using Stuff.Core;
using Stuff.Format;
using Stuff.Options;

namespace Stuff.Commands
{
    class Install : Command
    {
        public override string Name        => "install";
        public override string Description => "Downloads and installs stuff in .STUFF file(s)";

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("[directory|file|glob ...]");
            WriteLine("By default this will install all .STUFF files found in current directory.");

            WriteHead("Available options");
            WriteRow("-a, --all",             "Expand all if/else blocks -- everything is true");
            WriteRow("-f, --force",           "Don't early out if up-to-date, replace all directories");
            WriteRow("-D, --define=STRING",   "Add define");
            WriteRow("-U, --undefine=STRING", "Remove define");

            WriteDefines();
            WriteEnvironment();
        }

        public override void Execute(IEnumerable<string> args)
        {
            var flags = StuffFlags.None;
            var defines = new List<string>(StuffFile.DefaultDefines);
            var files = new OptionSet
                {
                    { "a|all", x => flags |= StuffFlags.AcceptAll },
                    { "f|force", x => flags |= StuffFlags.Force },
                    { "D=", defines.Add },
                    { "U=", x => defines.Remove(x) }
                }
                .Parse(args)
                .GetFiles("*.stuff");

            if (!Installer.InstallAll(files, flags, defines))
                throw new Exception("Install failed");
        }
    }
}
