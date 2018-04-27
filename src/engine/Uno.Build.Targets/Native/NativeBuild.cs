﻿using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.Extensions;

namespace Uno.Build.Targets.Native
{
    public class NativeBuild : BuildTarget
    {
        public override string Identifier => "Native";
        public override string FormerName => "CMake";
        public override string[] FormerNames => new[] {"CMake", "MSVC"};
        public override string Description => "C++/GL code, CMake project and native executable.";

        public override Backend CreateBackend()
        {
            return BackendFactory.NewCppBackend(BackendFactory.NewGLBackend(), new CppExtension());
        }

        public override void Configure(ICompiler compiler)
        {
            new CMakeGenerator(compiler.Environment).Configure();
        }
    }
}
