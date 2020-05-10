using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CIL;
using Uno.Compiler.Backends.OpenGL;

namespace Uno.Build.Targets
{
    public class DotNetBuild : BuildTarget
    {
        public override string Identifier => "DotNet";
        public override string FormerName => "DotNetExe";
        public override string Description => ".NET/GL bytecode and executable. (default)";
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return new CilBackend(new GLBackend());
        }
    }
}
