using Uno.Build.Targets.Generators;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Backends.CPlusPlus;
using Uno.Compiler.Backends.OpenGL;
using Uno.Compiler.Extensions;
using Uno.Diagnostics;

namespace Uno.Build.Targets
{
    public class iOSBuild : BuildTarget
    {
        public override string Identifier => "iOS";
        public override string Description => "(Objective-)C++/GLES2 code and Xcode project. (macOS only)";
        public override bool IsExperimental => !PlatformDetection.IsMac;

        public override Backend CreateBackend()
        {
            return new CppBackend(new GLBackend(), new CppExtension());
        }

        public override void Configure(ICompiler compiler)
        {
            XcodeGenerator.Configure(compiler.Environment, compiler.Data.Extensions.BundleFiles);
        }
    }
}
