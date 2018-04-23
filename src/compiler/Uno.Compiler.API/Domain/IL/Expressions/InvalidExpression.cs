using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class InvalidExpression : Expression
    {
        public InvalidExpression(DataType dt)
            : base(Source.Unknown)
        {
            ReturnType = dt ?? DataType.Invalid;
        }

        public InvalidExpression()
            : base(Source.Unknown)
        {
            ReturnType = DataType.Invalid;
        }

        public override DataType ReturnType { get; }

        public override ExpressionType ExpressionType => ExpressionType.Invalid;

        public override Expression CopyExpression(CopyState state)
        {
            return this;
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("<invalid>");
        }
    }
}