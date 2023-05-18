using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CIL;
using Uno.Compiler.Graphics.OpenGL;

namespace Uno.Build.Targets
{
    public class DotNetBuild : BuildTarget
    {
        public override string Identifier => "dotnet";
        public override string FormerName => "dotnetexe";
        public override string Description => ".NET/GL bytecode and executable. (default)";
        public override bool DefaultStrip => false;
        public override bool SupportsLazy => true;

        public override Backend CreateBackend()
        {
            return new CilBackend(new GLBackend());
        }

        public override void Initialize(IEnvironment env)
        {
            env.Define("DOTNET");
        }
    }
}
