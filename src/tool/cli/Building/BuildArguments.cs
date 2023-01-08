using Uno.Build;
using Uno.Logging;

namespace Uno.CLI.Building
{
    public struct BuildArguments
    {
        public Log Log;
        public BuildOptions Options;
        public BuildTarget Target;
        public string ProjectFile;
        public bool Run;
    }
}