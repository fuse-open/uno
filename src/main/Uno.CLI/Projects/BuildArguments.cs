using Uno.Build;
using Uno.Logging;

namespace Uno.CLI.Projects
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