using Uno.Build.Targets.Generators;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CPlusPlus;
using Uno.Compiler.Graphics.OpenGL;
using Uno.Compiler.Foreign;

namespace Uno.Build.Targets
{
    public class NativeBuild : BuildTarget
    {
        public override string Identifier => "native";
        public override string FormerName => "cmake";
        public override string[] FormerNames => new[] {"cmake", "msvc"};
        public override string ProjectGroup => "Native";
        public override string Description => "C++/GL code, CMake project and native executable.";

        public override Backend CreateBackend()
        {
            return new CppBackend(new GLBackend(), new ForeignExtension());
        }

        public override void Configure(ICompiler compiler)
        {
            new CMakeGenerator(compiler.Environment).Configure();
        }
    }
}
