using System;
using System.Threading;
using System.Threading.Tasks;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Diagnostics;
using Uno.Logging;
using Uno.ProjectFormat;

namespace Uno.Build
{
    public class BuildResult
    {
        public Log Log { get; private set; }
        public Project Project { get; private set; }
        public BuildTarget Target { get; private set; }
        public BuildFile File { get; }
        public ICompiler Compiler { get; }
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }
        public bool IsAborted { get; private set; }
        public string OutputDirectory { get; }
        public string RunArguments { get; }
        public BackendResult BackendResult { get; }
        public Method Entrypoint { get; private set; }
        public Namespace IL { get; private set; }
        public bool IsDebug { get; private set; }

        public BuildResult()
        {
        }

        internal BuildResult(Log log, Project project, BuildTarget target, ICompiler compiler, BuildOptions options, BuildFile file, BackendResult result)
        {
            Log = log;
            Project = project;
            Target = target;
            File = file;
            Compiler = compiler;
            ErrorCount = log.ErrorCount;
            WarningCount = log.WarningCount;
            OutputDirectory = compiler.Environment.OutputDirectory;
            RunArguments = options.RunArguments;
            BackendResult = result;
            IL = compiler.Data.IL;
            Entrypoint = compiler.Data.Entrypoint;
            IsDebug = compiler.Environment.Debug;
        }

        internal static BuildResult Error(Log log, Project project, BuildTarget target, bool aborted = false)
        {
            return new BuildResult
            {
                Log = log,
                Project = project,
                Target = target,
                IsAborted = aborted,
                ErrorCount = 1
            };
        }

        public void Run(Log log = null)
        {
            if (!Target.CanRun(File) ||
                    !Target.Run(GetShell(log), File, RunArguments).Result)
                throw new Exception(Target + " run failed");
        }

        public async Task RunAsync(Log log = null, CancellationToken ct = default)
        {
            if (!Target.CanRun(File) ||
                    ! await Target.Run(GetShell(log), File, RunArguments, ct))
                throw new Exception(Target + " run failed");
        }

        Shell GetShell(Log log)
        {
            return new Shell(log ?? Log);
        }
    }
}