namespace Uno.Compiler.API.Domain.AST.Expressions
{
    public enum AstCallType : byte
    {
        Function = AstExpressionType.Call,
        LookUp = AstExpressionType.LookUp,
    }
}