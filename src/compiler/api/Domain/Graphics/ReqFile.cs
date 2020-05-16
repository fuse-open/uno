namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class ReqFile : ReqStatement
    {
        public readonly string Filename;

        public override ReqStatementType Type => ReqStatementType.File;

        public ReqFile(Source src, string filename)
            : base(src)
        {
            Filename = filename;
        }

        public override string ToString()
        {
            return "req(" + Filename.ToLiteral() + ")";
        }
    }
}