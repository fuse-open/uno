using Uno.Compiler.API.Backends;

namespace Uno.Build.Targets.DotNet
{
    public class DotNetDllBuild : BuildTarget
    {
        public override string Identifier => "DotNetDll";
        public override string Description => ".NET/GL bytecode and library.";
        public override bool IsExperimental => true;
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return BackendFactory.NewCilBackend(BackendFactory.NewGLBackend());
        }
    }
}
