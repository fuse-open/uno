using Uno.Build.Targets.Generators;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CPlusPlus;
using Uno.Compiler.Backends.OpenGL;
using Uno.Compiler.Foreign;

namespace Uno.Build.Targets
{
    public class NativeBuild : BuildTarget
    {
        public override string Identifier => "Native";
        public override string FormerName => "CMake";
        public override string[] FormerNames => new[] {"CMake", "MSVC"};
        public override string Description => "C++/GL code, CMake project and native executable.";

        public override Backend CreateBackend()
        {
            return new CppBackend(new GLBackend(), new CppExtension());
        }

        public override void Configure(ICompiler compiler)
        {
            new CMakeGenerator(compiler.Environment).Configure();
        }
    }
}
