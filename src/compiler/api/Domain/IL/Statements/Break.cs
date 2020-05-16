namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class Break : Statement
    {
        public Break(Source src)
            : base(src)
        {
        }

        public override StatementType StatementType => StatementType.Break;

        public override Statement CopyStatement(CopyState state)
        {
            return this;
        }
    }
}