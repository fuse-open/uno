using Uno.Compiler.API.Backends;

namespace Uno.Build.Targets.DotNet
{
    public class DotNetBuild : BuildTarget
    {
        public override string Identifier => "DotNet";
        public override string FormerName => "DotNetExe";
        public override string Description => ".NET/GL bytecode and executable.";
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return BackendFactory.NewCilBackend(BackendFactory.NewGLBackend());
        }
    }
}
