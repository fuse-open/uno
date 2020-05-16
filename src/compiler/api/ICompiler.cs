using Uno.Compiler.API.Backends;
using Uno.Diagnostics;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.API
{
    public interface ICompiler
    {
        Log Log { get; }
        Disk Disk { get; }
        Shell Shell { get; }
        Backend Backend { get; }
        IBuildInput Input { get; }
        IBuildData Data { get; }
        IEnvironment Environment { get; }
        IILFactory ILFactory { get; }
        IUtilities Utilities { get; }
        IScheduler Scheduler { get; }
    }
}
