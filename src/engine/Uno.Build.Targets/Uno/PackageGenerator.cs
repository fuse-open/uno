using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Build.Packages;
using Uno.Collections;
using Uno.Compiler;
using Uno.Compiler.API;
using Uno.Compiler.Frontend;
using Uno.IO;
using Uno.Logging;

namespace Uno.Build.Targets.Uno
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
                    RequireFile(f.UnixPath);
                }
            }

            RequireFile(".uno/package");
            package.Save();

            // don't require this
            File.WriteAllText(
                Path.Combine(package.CacheDirectory, "config"), 
                _env.GetString("Configuration"));

            foreach (var p in _upk.References)
                _env.Require("Nuspec.DependencyElement", "<dependency id=\"" + p.Name + "\" version=\"" + p.Version + "\" />");
        }

        void RequireFile(string file)
        {
            // NuGet wants win paths on Windows; escape UXL macros
            file = file.UnixToNative().Replace("@(", "\\@(").Replace("\\\\@(", "\\\\\\@(");
            _env.Require("Nuspec.FileElement", "<file src=\"" + file + "\" target=\"" + file + "\" />");
        }
    }
}
