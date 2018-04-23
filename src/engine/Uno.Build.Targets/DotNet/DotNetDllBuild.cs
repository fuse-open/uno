using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CIL;
using Uno.Compiler.Backends.OpenGL;

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
            return new CilBackend(new GLBackend());
        }
    }
}
