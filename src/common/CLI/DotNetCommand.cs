using System;
using System.Collections.Generic;
using System.Reflection;
using Uno.IO;

namespace Uno.CLI
{
    public abstract class DotNetCommand : Command
    {
        public abstract string Exe { get; }

        public override void Help(IEnumerable<string> args)
        {
            LoadAndRun(Exe, GetAllArguments(args, false, "--help").ToArray());
        }

        public override void Execute(IEnumerable<string> args)
        {
            ExitCode = LoadAndRun(Exe, GetAllArguments(args).ToArray());
        }

        public static int LoadAndRun(string filename, params string[] args)
        {
            var asm = Assembly.LoadFrom(filename);

            foreach (var type in asm.GetTypes())
            {
                var main = type.GetMethod(
                        "Main", 
                        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, 
                        null, 
                        new[] {typeof(string[])}, 
                        null);
                if (main == null)
                    continue;

                var result = main.Invoke(null, new object[] {args});
                return result as int? ?? 0;
            }

            throw new ArgumentException(filename.ToRelativePath() + ": A suitable entrypoint was not found.");
        }
    }
}
