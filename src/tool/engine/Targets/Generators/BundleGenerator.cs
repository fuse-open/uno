using System.IO;
using Uno.Build.Libraries;
using Uno.Compiler;
using Uno.Compiler.API;
using Uno.Compiler.Frontend;
using Uno.IO;
using Uno.Logging;

namespace Uno.Build.Targets.Generators
{
    class BundleGenerator
    {
        readonly IEnvironment _env;
        readonly SourceBundle _bundle;
        readonly Log _log;
        readonly Disk _disk;

        public BundleGenerator(
            IEnvironment env,
            SourceBundle bundle,
            Log log,
            Disk disk)
        {
            _env = env;
            _bundle = bundle;
            _log = log;
            _disk = disk;
        }

        public void Generate()
        {
            var manifest = new ManifestFile(_bundle, _env.OutputDirectory);
            new SourceReader(_log, _bundle, _env)
                .ExportCache(
                    manifest.CacheDirectory,
                    manifest.ExtensionsBackends,
                    manifest.Namespaces);

            if (_env.Debug)
            {
                // DEBUG: Reuse source files from project directory
                manifest.SourceDirectory = _bundle.SourceDirectory;
            }
            else
            {
                // RELEASE: Copy source files into bundle
                foreach (var f in _bundle.AllFiles)
                {
                    var source = Path.Combine(_bundle.SourceDirectory, f.NativePath);
                    var destination = Path.Combine(manifest.RootDirectory, f.NativePath);
                    _disk.CopyFile(source, destination);
                }
            }

            manifest.Save();
            File.WriteAllText(
                Path.Combine(manifest.CacheDirectory, "config"),
                _env.GetString("configuration"));
        }
    }
}
