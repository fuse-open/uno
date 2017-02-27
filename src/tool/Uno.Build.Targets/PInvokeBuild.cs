using Uno.Build.Targets.Generators;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.PInvoke;

namespace Uno.Build.Targets
{
    public class PInvokeBuild : BuildTarget
    {
        public override string Identifier => "PInvoke";
        public override string Description => "PInvoke libraries.";
        public override bool IsExperimental => true;
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return new PInvokeBackend(ShaderBackend.Dummy) { BuildType = BuildType.Library };
        }

        public override void Configure(ICompiler compiler)
        {
            new CMakeGenerator(compiler.Environment).Configure();
        }
    }
}
