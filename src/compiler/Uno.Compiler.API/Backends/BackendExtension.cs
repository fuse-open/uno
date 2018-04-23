using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Backends
{
    public abstract class BackendExtension
    {
        public static readonly BackendExtension Null = new NullExtension();

        protected Backend Backend { get; private set; }
        protected ICompiler Compiler { get; private set; }
        protected IBuildData Data { get; private set; }
        protected IBuildInput Input { get; private set; }

        public void Begin(ICompiler compiler)
        {
            Backend = compiler.Backend;
            Compiler = compiler;
            Data = compiler.Data;
            Input = compiler.Input;

            Initialize();
            Begin();
        }

        protected virtual void Initialize()
        {
        } 

        protected abstract void Begin();

        protected void AddGenerator(Pass pass)
        {
            Compiler.Scheduler.AddGenerator(pass);
        }

        protected void AddTransform(Pass pass)
        {
            Compiler.Scheduler.AddTransform(pass);
        }

        protected bool IsDefined(string def)
        {
            return Compiler.Environment.IsDefined(def);
        }

        class NullExtension : BackendExtension
        {
            protected override void Begin()
            {
            }
        }
    }
}