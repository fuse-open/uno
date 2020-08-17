using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CIL;
using Uno.Compiler.Graphics.OpenGL;

namespace Uno.Build.Targets
{
    public class MetadataBuild : BuildTarget
    {
        public override string Identifier => "metadata";
        public override string ProjectGroup => "Metadata";
        public override string Description => "Metadata for code completion.";
        public override bool IsExperimental => true;
        public override bool DefaultStrip => false;

        public override Backend CreateBackend()
        {
            return new MetadataBackend(new GLBackend());
        }
    }
}
