using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Uno.CLI.Loader
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var dir = Path.GetDirectoryName(typeof (Program).Assembly.Location);
                foreach (var f in EnumerateDlls(dir))
                    return LoadAndRun(f, "Uno.CLI.Program", args);

                var stuff = Path.Combine(dir, "stuff.exe");
                var stuffFile = Path.Combine(dir, "uno.stuff");
                if (File.Exists(stuff) && File.Exists(stuffFile))
                {
                    // Download Uno, and try again
                    LoadAndRun(stuff, "Stuff.Program", "install", stuffFile);
                    foreach (var f in EnumerateDlls(dir))
                        return LoadAndRun(f, "Uno.CLI.Program", args);
                }

                Console.Error.WriteLine("FATAL ERROR: 'Uno.CLI.dll' was not found -- please check your '.unopath' file");
                return 1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("FATAL ERROR: " + e);
                return 1;
            }
        }

        static IEnumerable<string> EnumerateDlls(string dir)
        {
            foreach (var l in File.ReadAllText(Path.Combine(dir, ".unopath")).Split('\n'))
            {
                var p = l.Trim();
                if (p.Length == 0)
                    continue;

                var f = Path.Combine(dir, p, "Uno.CLI.dll");
                if (File.Exists(f))
                    yield return f;
            }
        }

        static int LoadAndRun(string filename, string program, params string[] args)
        {
            var asm = Assembly.LoadFrom(filename);
            var type = asm.GetType(program, true);
            var method = type.GetMethod("Main");
            return (int) method.Invoke(null, new object[] {args});
        }
    }
}
