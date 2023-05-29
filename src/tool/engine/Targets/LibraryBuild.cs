using Uno.Build.Targets.Generators;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;

namespace Uno.Build.Targets
{
    public class LibraryBuild : BuildTarget
    {
        public override string Identifier => "library";
        public override string FormerName => "package";
        public override string Description => "Uno library bundle.";
        public override bool IsExperimental => true;
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return new DefaultBackend { BuildType = BuildType.Library };
        }

        public override void Initialize(IEnvironment env)
        {
            env.Define("LIBRARY");
        }

        public override void Configure(ICompiler compiler)
        {
            new BundleGenerator(
                    compiler.Environment,
                    compiler.Input.Bundle,
                    compiler.Log,
                    compiler.Disk)
                .Generate();
        }
    }
}
