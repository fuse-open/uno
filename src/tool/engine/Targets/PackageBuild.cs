using Uno.Build.Targets.Generators;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;

namespace Uno.Build.Targets
{
    public class PackageBuild : BuildTarget
    {
        public override string Identifier => "package";
        public override string FormerName => "unopackage";
        public override string ProjectGroup => "Package";
        public override string Description => "Uno package files.";
        public override bool IsExperimental => true;
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return new DefaultBackend { BuildType = BuildType.Library };
        }

        public override void Configure(ICompiler compiler)
        {
            new PackageGenerator(
                    compiler.Environment,
                    compiler.Input.Package,
                    compiler.Log,
                    compiler.Disk)
                .Generate();
        }
    }
}
