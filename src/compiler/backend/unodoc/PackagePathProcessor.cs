using System.Collections.Generic;
using Uno.Logging;

namespace Uno.Compiler.Backends.UnoDoc
{
    internal class PackagePathProcessor
    {
        Log log;
        IReadOnlyList<SourcePackage> _packages;
        readonly Dictionary<string, string> _packagePaths = new Dictionary<string, string>();

        public PackagePathProcessor(Log log, IReadOnlyList<SourcePackage> _packages)
        {
            this.log = log;
            this._packages = _packages;
            BuildPackagePaths();
        }

        void BuildPackagePaths()
        {
            foreach (var p in _packages)
            {
                _packagePaths.Add(p.Name, p.SourceDirectory);
            }
        }

        public string GetPathFor(string package)
        {
            string path;
            if (_packagePaths.TryGetValue(package, out path)) {
                return path;
            } else return null;
        }
    }
}