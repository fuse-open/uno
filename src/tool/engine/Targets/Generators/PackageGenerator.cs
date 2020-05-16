using System.IO;
using Uno.Build.Packages;
using Uno.Compiler;
using Uno.Compiler.API;
using Uno.Compiler.Frontend;
using Uno.IO;
using Uno.Logging;

namespace Uno.Build.Targets.Generators
{
    class PackageGenerator
    {
        readonly IEnvironment _env;
        readonly SourcePackage _upk;
        readonly Log _log;
        readonly Disk _disk;

        public PackageGenerator(
            IEnvironment env,
            SourcePackage upk,
            Log log,
            Disk disk)
        {
            _env = env;
            _upk = upk;
            _log = log;
            _disk = disk;
        }

        public void Generate()
        {
            var package = new PackageFile(_upk, _env.OutputDirectory);
            new SourceReader(_log, _upk, _env)
                .ExportCache(
                    package.CacheDirectory,
                    package.ExtensionsBackends,
                    package.Namespaces);

            if (_env.Debug)
            {
                // DEBUG: Reuse source files from project directory
                package.SourceDirectory = _upk.SourceDirectory;
            }
            else
            {
                // RELEASE: Copy source files into package
                foreach (var f in _upk.AllFiles)
                {
                    var source = Path.Combine(_upk.SourceDirectory, f.NativePath);
                    var destination = Path.Combine(package.RootDirectory, f.NativePath);
                    _disk.CopyFile(source, destination);
                }
            }

            package.Save();
            File.WriteAllText(
                Path.Combine(package.CacheDirectory, "config"), 
                _env.GetString("Configuration"));
        }
    }
}
