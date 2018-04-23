namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstModifiedStatement : AstStatement
    {
        public readonly AstStatementModifier Modifier;
        public readonly AstStatement Statement;

        public override AstStatementType StatementType => (AstStatementType)Modifier;

        public AstModifiedStatement(Source src, AstStatementModifier modifier, AstStatement statement)
            : base(src)
        {
            Modifier = modifier;
            Statement = statement;
        }
    }
}