using System.Collections.Generic;
using Uno.Build;

namespace Uno.TestRunner
{
    public class TestOptions
    {
        public List<string> Paths;
        public string LogFileName;
        public BuildTarget Target;
        public bool Verbose;
        public string Filter;
        public bool Trace;
        public bool OnlyBuild;
        public bool OnlyGenerate;
        public bool NoUninstall;
        public bool Library;
        public string OutputDirectory;
        public readonly List<string> Defines = new List<string>();
        public readonly List<string> Undefines = new List<string>();
    }
}
