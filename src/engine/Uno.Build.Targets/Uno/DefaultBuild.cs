using Uno.Compiler.API.Backends;

namespace Uno.Build.Targets.Uno
{
    public class DefaultBuild : BuildTarget
    {
        public override string Identifier => "(default)";
        public override string Description => "Produces nothing. Use for validation.";
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return new DefaultBackend {BuildType = BuildType.Library};
        }
    }
}
