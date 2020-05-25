using System.Collections.Generic;
using Uno.Build.Adb;

namespace Uno.CLI.Android
{
    class Adb : Command
    {
        public override string Name => "adb";
        public override string Description => "Use Android Debug Bridge (adb).";
        public override bool IsExperimental => true;

        public override void Help()
        {
            WriteUsage("[arguments ...]");
            WriteLine("This commands forwards given arguments to 'adb', which is a tool included in the Android SDK.");
            WriteLine("\nType 'uno adb' to see what's available.");
        }

        public override void Execute(IEnumerable<string> args)
        {
            new AdbRunner(Shell).Run(GetAllArguments(args).JoinArguments());
        }
    }
}
