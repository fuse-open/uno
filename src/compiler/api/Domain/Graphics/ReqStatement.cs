namespace Uno.Compiler.API.Domain.Graphics
{
    public abstract class ReqStatement : SourceObject
    {
        public abstract ReqStatementType Type { get; }

        protected ReqStatement(Source src)
            : base(src)
        {
        }
    }
}