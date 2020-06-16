using System.Collections.Generic;
using Uno.Build.Packages;

namespace Uno.Build
{
    public class BuildOptions
    {
        public BuildConfiguration Configuration = BuildConfiguration.Debug;
        public bool Force;
        public bool NativeBuild = true;
        public bool Parallel = true;
        public bool? UpdateLibrary;
        public bool? Strip;
        public bool Clean;
        public bool Test;
        public bool PrintInternals;
        public int WarningLevel = 2;
        public int MaxErrorCount = 500;
        public string OutputDirectory;
        public string MainClass;
        public string TestFilter;
        public string NativeArguments;
        public string RunArguments;
        public readonly List<string> Defines = new List<string>();
        public readonly List<string> Undefines = new List<string>();
        public readonly Dictionary<string, string> Settings = new Dictionary<string, string>();
        public PackageCache PackageCache;

        public override int GetHashCode()
        {
            var hash = 27;
            hash = 3 * hash + Configuration.GetHashCode();
            hash = 3 * hash + UpdateLibrary.NullableHashCode();
            hash = 3 * hash + Strip.NullableHashCode();
            hash = 2 * hash + Test.GetHashCode();
            hash = 13 * hash + MainClass.NullableHashCode();
            hash = 13 * hash + TestFilter.NullableHashCode();

            foreach (var d in Defines)
                hash = 13 * hash + d.GetHashCode();
            hash <<= 1;

            foreach (var d in Undefines)
                hash = 13 * hash + d.GetHashCode();
            hash <<= 1;

            foreach (var d in Settings)
                hash = 13 * hash + d.Key.GetHashCode() + d.Value.GetHashCode();
            return hash;
        }
    }
}
