using Uno.Build.Targets.Native;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;

namespace Uno.Build.Targets.PInvoke
{
    public class PInvokeBuild : BuildTarget
    {
        public override string Identifier => "PInvoke";
        public override string Description => "PInvoke libraries.";
        public override bool IsExperimental => true;
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            var backend = BackendFactory.NewPInvokeBackend(ShaderBackend.Dummy);
            backend.BuildType = BuildType.Library;
            return backend;
        }

        public override void Configure(ICompiler compiler)
        {
            new CMakeGenerator(compiler.Environment).Configure();
        }
    }
}
