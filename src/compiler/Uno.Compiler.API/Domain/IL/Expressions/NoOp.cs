using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class NoOp : VoidExpression
    {
        public readonly string Comment;

        public NoOp(Source src, string comment = null)
            : base(src)
        {
            Comment = comment;
        }

        public override ExpressionType ExpressionType => ExpressionType.NoOp;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("/* " + (Comment ?? "NoOp") + " */");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return this;
        }
    }
}