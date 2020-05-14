namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstEmptyStatement : AstStatement
    {
        public readonly AstEmptyStatementType Type;

        public override AstStatementType StatementType => (AstStatementType) Type;

        public AstEmptyStatement(Source src, AstEmptyStatementType type)
            : base(src)
        {
            Type = type;
        }
    }
}