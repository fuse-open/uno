using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Options;
using Uno.Build.Packages;
using Uno.Configuration;
using Uno.IO;

namespace Uno.CLI.Diagnostics
{
    class Config : Command
    {
        public override string Name => "config";
        public override string Description => "Print information about your Uno configuration.";

        public override void Help()
        {
            WriteUsage("[property ...]", "[options]");

            WriteHead("Available options");
            WriteRow("-a, --asm",           "Print .NET assemblies");
            WriteRow("-e, --env",           "Print environment variables");
            WriteRow("-l, --libs",          "Print libraries in search paths");
            WriteRow("-n, --node-modules",  "Print nodejs modules");
            WriteRow("-vv",                 "Print everything");
            WriteRow("-v",                  "Verbose mode");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var asm = false;
            var libs = false;
            var node = false;
            var system = false;
            
            var input = new OptionSet {
                    { "a|asm", value => asm = true },
                    { "e|env", value => system = true },
                    { "l|libs", value => libs = true },
                    { "n|node-modules", value => node = true },
                    { "s|sys|system", value => system = true }  // legacy
                }.Parse(args);

            if (input.Count > 0)
            {
                input.CheckArguments();
                foreach (var key in input)
                    WriteLine(UnoConfig.Current.GetString(key) ?? "(null)");
                return;
            }

            if (Log.IsVerbose)
                WriteProductInfo();
            else
                Log.ProductHeader();

            if (!asm && !libs && !node && !system || Log.IsVerbose)
            {
                WriteHead("Uno settings", indent: 0);
                WriteRow(UnoConfig.Current);

                WriteHead("Config files", indent: 0);
                foreach (var f in UnoConfig.Current.GetFilenames(Log.IsVerbose))
                    WriteRow(f.ToRelativePath());

                WriteHead("Config defines", indent: 0);
                var defines = UnoConfigFile.Defines.ToArray();
                Array.Sort(defines);
                WriteLine(string.Join(" ", defines));
            }

            if (asm || Log.IsVerbose)
            {
                WriteHead(".NET assemblies", indent: 0);
                var configAssembly = typeof (Config).Assembly;
                var configVersion = configAssembly.GetName().Version;
                foreach (var f in GetDotNetAssemblies(configAssembly))
                    if (f.GetName().Version != configVersion)
                        WriteRow(f.Location.ToRelativePath() + " (" + f.GetName().Version + ")");
            }

            if (libs || Log.IsVeryVerbose)
            {
                WriteHead("Uno libraries", 28, 0);
                foreach (var lib in GetUnoLibraries())
                    WriteRow(lib.Name, lib.Location.ToRelativePath(), parse: false);
            }

            if (node || Log.IsVeryVerbose)
            {
                WriteHead("Node.js modules", 28, 0);
                foreach (var module in UnoConfig.Current.NodeModules)
                    WriteRow(module.Key, module.Value, parse: false);
            }

            if (system || Log.IsVeryVerbose)
            {
                WriteHead("Environment variables", 24, 0);
                foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
                    WriteRow(e.Key, e.Value, parse: false);
            }
        }

        // Not really correct, but good enough
        // http://stackoverflow.com/questions/2384592/is-there-a-way-to-force-all-referenced-assemblies-to-be-loaded-into-the-app-doma
        static IReadOnlyList<Assembly> GetDotNetAssemblies(Assembly assembly)
        {
            var assemblyDir = Path.GetDirectoryName(assembly.Location);
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();
            var referencedPaths = Directory.GetFiles(assemblyDir, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));
            return loadedAssemblies;
        }

        static IReadOnlyList<Library> GetUnoLibraries()
        {
            var cache = new PackageCache();
            var list = new List<Library>();
            var set = new HashSet<string>();

            foreach (var directory in cache.SearchPaths)
            {
                foreach (var package in cache.GetPackageDirectories(directory).Keys)
                {
                    foreach (var versionDir in cache.GetVersionDirectories(package))
                    {
                        if (PackageFile.Exists(versionDir.FullName) && !set.Contains(versionDir.FullName))
                        {
                            list.Add(new Library(package, versionDir.FullName));
                            set.Add(versionDir.FullName);
                        }
                    }
                }
            }

            list.Sort();
            return list;
        }

        class Library : IComparable<Library>
        {
            public string Name { get; }
            public string Location { get; }

            public Library(string name, string dir)
            {
                Name = name;
                Location = dir;
            }

            public int CompareTo(Library other)
            {
                return string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
