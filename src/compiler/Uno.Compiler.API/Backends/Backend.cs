using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Utilities;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.API.Backends
{
    public abstract class Backend : IKeywords
    {
        public abstract string Name { get; }

        public BuildType BuildType { get; set; }
        protected BackendOptions Options { get; set; }
        protected TypeOptions TypeOptions { get; set; }
        protected FunctionOptions FunctionOptions { get; set; }

        public Log Log { get; private set; }
        public Disk Disk { get; private set; }
        public ShaderBackend ShaderBackend { get; private set; }
        public Decompiler Decompiler { get; protected set; }
        protected internal IBuildData Data { get; private set; }
        protected internal IBuildInput Input { get; private set; }
        protected internal IEnvironment Environment { get; private set; }
        protected internal IEssentials Essentials { get; private set; }
        protected internal IILFactory ILFactory { get; private set; }
        protected internal IScheduler Scheduler { get; private set; }
        protected internal IUtilities Utilities { get; private set; }

        public bool AllowInvalidCode => Options.HasFlag(BackendOptions.AllowInvalidCode);
        public bool CanExportDontExports => Options.HasFlag(BackendOptions.ExportDontExports);
        public bool IsDefault => this is DefaultBackend;

        protected Backend(ShaderBackend shaderBackend = null)
        {
            ShaderBackend = shaderBackend ?? ShaderBackend.Dummy;
            Decompiler = new Decompiler();
        }

        public bool Has(TypeOptions f)
        {
            return TypeOptions.HasFlag(f);
        }

        public bool Has(FunctionOptions f)
        {
            return FunctionOptions.HasFlag(f);
        }

        public virtual void Begin(ICompiler compiler)
        {
            Log = compiler.Log;
            Disk = compiler.Disk;
            Data = compiler.Data;
            Input = compiler.Input;
            Environment = compiler.Environment;
            Essentials = compiler.ILFactory.Essentials;
            ILFactory = compiler.ILFactory;
            Scheduler = compiler.Scheduler;
            Utilities = compiler.Utilities;
        }

        public virtual void End()
        {
        }

        public virtual void Configure()
        {
        }

        public virtual void BeginBuild()
        {
        }

        public virtual void EndBuild()
        {
        }

        public virtual BackendResult Build()
        {
            BackendResult result = null;
            foreach (var package in Input.Packages)
                result = Build(package);
            return result;
        }

        public virtual BackendResult Build(SourcePackage package)
        {
            return null;
        }

        public virtual bool CanLink(SourcePackage upk)
        {
            return false;
        }

        public virtual bool CanLink(DataType type)
        {
            return false;
        }

        public virtual bool CanLink(Function function)
        {
            return false;
        }

        public virtual bool PassByRef(Function function)
        {
            return false;
        }

        public virtual bool IsConstrained(Function function)
        {
            return false;
        }

        public virtual bool IsReserved(string identifier)
        {
            return false;
        }

        public sealed override string ToString()
        {
            return Name;
        }
    }
}
