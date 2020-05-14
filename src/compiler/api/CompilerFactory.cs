using System;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API
{
    public static class CompilerFactory
    {
        static Func<Function, IBytecodeCompiler> _implementation;

        public static void Initialize(Func<Function, IBytecodeCompiler> implementation)
        {
            _implementation = implementation;
        }

        public static IBytecodeCompiler CreateBytecodeCompiler(this Function function)
        {
            return _implementation(function);
        }
    }
}