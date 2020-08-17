using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Uno.CLI;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Configuration;
using Uno.Diagnostics;
using Uno.IO;

namespace Uno.Build
{
    public abstract class BuildTarget
    {
        public abstract string Identifier { get; }
        public abstract string ProjectGroup { get; }
        public virtual string FormerName => "";
        public virtual string[] FormerNames => new[] {FormerName};
        public virtual string Description => "";
        public virtual bool IsExperimental => false;
        public virtual bool IsObsolete => false;
        public virtual bool DefaultStrip => true;

        public virtual Backend CreateBackend()
        {
            throw new NotImplementedException();
        }

        public virtual Backend CreateBackend(UnoConfig config)
        {
            return CreateBackend();
        }

        public virtual void Initialize(IEnvironment env)
        {
        }

        public virtual void Configure(ICompiler compiler)
        {
        }

        public virtual void DeleteOutdated(Disk disk, IEnvironment env)
        {
        }

        public virtual bool CanBuild(BuildFile file)
        {
            return !string.IsNullOrEmpty(file.BuildCommand);
        }

        public virtual bool CanRun(BuildFile file)
        {
            return !string.IsNullOrEmpty(file.RunCommand);
        }

        public virtual bool Build(Shell shell, BuildFile file, string args = null)
        {
            var filename = string.Concat(file.BuildCommand, " ", args).SplitCommand(file.RootDirectory, out args);
            return shell.Run(filename, args, file.RootDirectory, RunFlags.Colors | RunFlags.Compact) == 0;
        }

        public virtual async Task<bool> Run(Shell shell, BuildFile file, string args = null, CancellationToken ct = default(CancellationToken))
        {
            var filename = string.Concat(file.RunCommand, " ", args).SplitCommand(file.RootDirectory, out args);
            return await shell.Start(filename, args, file.RootDirectory, 0, null, ct) == 0;
        }

        public override string ToString()
        {
            return Identifier;
        }
    }
}
