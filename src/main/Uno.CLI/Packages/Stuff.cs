using System.Collections.Generic;

namespace Uno.CLI.Packages
{
    class Stuff : Command
    {
        public override string Name => "stuff";
        public override string Description => "Built-in simple package manager.";
        public override bool IsExperimental => true;

        public override void Help(IEnumerable<string> args)
        {
            new global::Stuff.Program().Execute(GetAllArguments(args, false, "--help"));
        }

        public override void Execute(IEnumerable<string> args)
        {
            new global::Stuff.Program().Execute(GetAllArguments(args));
        }

        static Stuff()
        {
            // Override Stuff™ root command name
            global::Stuff.Command.Root = "uno stuff";
        }
    }
}
