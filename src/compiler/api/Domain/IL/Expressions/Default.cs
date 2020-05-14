using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class Default : Expression
    {
        public DataType ValueType;

        public override DataType ReturnType => ValueType;

        public Default(Source src, DataType dt)
            : base(src)
        {
            ValueType = dt;
        }

        public override ExpressionType ExpressionType => ExpressionType.Default;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("default(");
            sb.Append(ValueType);
            sb.Append(")");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new Default(Source, state.GetType(ValueType));
        }
    }
}