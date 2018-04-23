using Uno.Compiler.API.Domain.AST.Statements;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public enum VariableType
    {
        Default,
        Constant = AstVariableModifier.Const,
        Extern = AstVariableModifier.Extern,
        Iterator,
        Exception,
        Indirection
    }
}