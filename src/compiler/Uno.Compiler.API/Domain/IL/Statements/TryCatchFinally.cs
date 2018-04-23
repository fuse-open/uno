using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public class TryCatchFinally : Statement
    {
        public Scope TryBody;
        public Scope OptionalFinallyBody;
        public readonly CatchBlock[] CatchBlocks;

        public TryCatchFinally(Source src, Scope tryBody, Scope optionalFinallyBody, params CatchBlock[] gotchas)
            : base(src)
        {
            TryBody = tryBody;
            OptionalFinallyBody = optionalFinallyBody;
            CatchBlocks = gotchas;
        }

        public override StatementType StatementType => StatementType.TryCatchFinally;

        public override void Visit(Pass p, ExpressionUsage u)
        {
            TryBody.Visit(p);
            p.Next(this);

            foreach (var c in CatchBlocks)
                c.Body.Visit(p);

            OptionalFinallyBody?.Visit(p);
        }

        public override Statement CopyStatement(CopyState state)
        {
            return new TryCatchFinally(Source, TryBody.CopyNullable(state), OptionalFinallyBody.CopyNullable(state), CatchBlocks.Copy(state));
        }
    }
}