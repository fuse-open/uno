namespace Uno.Compiler.API.Domain.AST.Statements
{
    public sealed class AstUsing : AstStatement
    {
        public readonly AstStatement Initializer;
        public readonly AstStatement OptionalBody;

        public override AstStatementType StatementType => AstStatementType.Using;

        public AstUsing(Source src, AstStatement initializer, AstStatement optionalBody)
            : base(src)
        {
            Initializer = initializer;
            OptionalBody = optionalBody;
        }
    }
}