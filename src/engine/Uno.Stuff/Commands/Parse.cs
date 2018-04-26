using System.Collections.Generic;
using Stuff.Format;
using Mono.Options;

namespace Stuff.Commands
{
    class Parse : Command
    {
        public override string Name        => "parse";

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("[directory|file|glob ...]");

            WriteHead("Available options");
            WriteRow("-a, --all",             "Expand all if/else blocks -- everything is true");
            WriteRow("-D, --define=STRING",   "Add define");
            WriteRow("-U, --undefine=STRING", "Remove define");
            WriteDefines();
        }

        public override void Execute(IEnumerable<string> args)
        {
            var flags = StuffFlags.Print;
            var defines = new List<string>(StuffFile.DefaultDefines);
            var files = new OptionSet
                {
                    { "a|all", x => flags |= StuffFlags.AcceptAll },
                    { "D=", defines.Add },
                    { "U=", x => defines.Remove(x) }
                }
                .Parse(args)
                .GetFiles("*.stuff");

            foreach (var file in files)
                StuffObject.Load(file, flags, defines);
        }
    }
}
