using System.Collections.Generic;

namespace Uno.TestGenerator
{
    class Options
    {
        public readonly string PackageDir;
        public readonly string TempProj;
        public readonly List<string> Exlcudes;

        public Options(string packageDir, string tempProj, List<string> exlcudes)
        {
            PackageDir = packageDir;
            TempProj = tempProj;
            Exlcudes = exlcudes;
        }
    }
}