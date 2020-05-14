namespace Uno.Compiler.API.Domain.AST.Statements
{
    public enum AstStatementModifier : byte
    {
        AutoRelease = AstStatementType.AutoRelease,
        Unsafe = AstStatementType.Unsafe,
        Unchecked = AstStatementType.Unchecked,
        Checked = AstStatementType.Checked,
    }
}