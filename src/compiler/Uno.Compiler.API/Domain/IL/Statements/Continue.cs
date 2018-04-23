namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class Continue : Statement
    {
        public Continue(Source src)
            : base(src)
        {
        }

        public override StatementType StatementType => StatementType.Continue;

        public override Statement CopyStatement(CopyState state)
        {
            return this;
        }
    }
}