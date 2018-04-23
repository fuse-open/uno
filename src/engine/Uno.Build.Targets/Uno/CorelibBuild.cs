using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CSharp;

namespace Uno.Build.Targets.Uno
{
    public class CorelibBuild : BuildTarget
    {
        public override string Identifier => "corelib";
        public override string FormerName => "UnoCore";
        public override string Description => "C# implementation of Uno corelib.";
        public override bool IsExperimental => true;
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return new CsBackend { BuildType = BuildType.Library };
        }
    }
}
