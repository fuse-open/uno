using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Options;
using Uno.Configuration;
using Uno.IO;

namespace Uno.CLI.Diagnostics
{
    class Config : Command
    {
        public override string Name => "config";
        public override string Description => "Print information about your Uno environment.";

        public override void Help()
        {
            WriteUsage("[property ...]", "[options]");

            WriteHead("Available options");
            WriteRow("-a, --asm",          "Print .NET assemblies");
            WriteRow("-s, --system",       "Print system settings");
            WriteRow("-v",                 "Print everything");
        }

        public override void Execute(IEnumerable<string> args)
        {
            Stuff.Log.Configure(Log.IsVeryVerbose, Log.OutWriter, Log.ErrorWriter);

            var asm = false;
            var system = false;
            var input = new OptionSet {
                    { "a|asm", value => asm = true },
                    { "s|sys|system", value => system = true }
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

            WriteHead("Uno settings", indent: 0);
            WriteRow(UnoConfig.Current);

            WriteHead("Config files", indent: 0);
            var files = new List<string>();

            foreach (var f in UnoConfig.Current.Files)
            {
                files.Add(f.Stuff.Filename);
                f.Stuff.Flatten(
                    filename =>
                    {
                        if (!files.Contains(filename) || Log.IsVerbose)
                            files.Add(filename);
                        return File.ReadAllText(filename);
                    });
            }

            foreach (var f in files)
                WriteRow(f.ToRelativePath());

            if (asm || Log.IsVerbose)
            {
                WriteHead(".NET assemblies", indent: 0);
                var configAssembly = typeof (Config).Assembly;
                var configVersion = configAssembly.GetName().Version;
                foreach (var f in GetAllAssemblies(configAssembly))
                    if (f.GetName().Version != configVersion)
                        WriteRow(f.Location.ToRelativePath() + " (" + f.GetName().Version + ")");
            }

            if (system || Log.IsVerbose)
            {
                WriteHead("System settings", 24, 0);
                foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
                    WriteRow(e.Key, e.Value, parse: false);
            }
        }

        // Not really correct, but good enough
        // http://stackoverflow.com/questions/2384592/is-there-a-way-to-force-all-referenced-assemblies-to-be-loaded-into-the-app-doma
        static IReadOnlyList<Assembly> GetAllAssemblies(Assembly assembly)
        {
            var assemblyDir = Path.GetDirectoryName(assembly.Location);
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();
            var referencedPaths = Directory.GetFiles(assemblyDir, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));
            return loadedAssemblies;
        }
    }
}
