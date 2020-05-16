using System.Collections.Generic;
using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API
{
    public interface IBytecodeCompiler
    {
        Function Function { get; }
        List<Instruction> Code { get; }
        List<Label> Labels { get; }
        List<Variable> Locals { get; }

        void Compile();
    }
}