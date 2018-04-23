using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public enum FixOpType
    {
        IncreaseBefore = AstUnaryType.IncreasePrefix,
        IncreaseAfter = AstUnaryType.IncreasePostfix,
        DecreaseBefore = AstUnaryType.DecreasePrefix,
        DecreaseAfter = AstUnaryType.DecreasePostfix
    }
}