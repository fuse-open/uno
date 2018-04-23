using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Options;
using Uno.TestGenerator.Properties;

namespace Uno.TestGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--help")
            {
                Console.WriteLine("Usage: uno test-gen <path to packages> <path for temporary project> [--exclude=name]");
                return;
            }

            var options = ParseOptions(args);
            var packageNames =
                Directory.GetDirectories(options.PackageDir)
                    .Select(Path.GetFileName)
                    .Where(packageName =>
                        !packageName.StartsWith("_") &&
                        packageName != "build" &&
                        !options.Exlcudes.Contains(packageName))
                    .ToList();

            foreach (var packageName in packageNames)
                Console.WriteLine("Including package " + packageName);

            var references = from pkg in packageNames select string.Format("        \"{0}\"", pkg);
            var referenceString = string.Join(",\r\n", references);
            Directory.CreateDirectory(options.TempProj);
            File.WriteAllText(Path.Combine(options.TempProj, "pkgtest.unoproj"), Resources.Template.Replace("{{References}}", referenceString));
            File.WriteAllText(Path.Combine(options.TempProj, "MainClass.uno"), Resources.MainClass);
            Console.WriteLine();
            Console.WriteLine(@"Finished generating " + options.TempProj);
        }

        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private static Options ParseOptions(string[] args)
        {
            var excludes = new List<string>();
            var p = new OptionSet
            {
                {"exclude=", "Exclude directory with this name", v => excludes.Add(v.Trim('\"'))}
            };
            var extra = p.Parse(args);
            if (extra.Count != 2)
            {
                Console.Error.WriteLine("Usage: uno test-gen <path to packages> <path for temporary project> [--exclude=name]");
                Environment.Exit(1);
            }
            return new Options(extra[0], extra[1], excludes);
        }

    }
}
