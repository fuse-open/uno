using System.Collections.Generic;
using Stuff.Core;

namespace Stuff.Commands
{
    class Clean : Command
    {
        public override string Name        => "clean";
        public override string Description => "Removes stuff installed by .STUFF file(s)";

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("[directory|file|glob ...]");
            WriteLine("By default this will clean all .STUFF files found in current directory.");
        }

        public override void Execute(IEnumerable<string> args)
        {
            Installer.CleanAll(args.GetFiles("*.stuff"));
        }
    }
}