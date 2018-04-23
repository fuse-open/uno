using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class Base : Expression
    {
        public DataType BaseType;

        public override DataType ReturnType => BaseType;

        public Base(Source src, DataType dt)
            : base(src)
        {
            BaseType = dt;
        }

        public override ExpressionType ExpressionType => ExpressionType.Base;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("base");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new Base(Source, state.GetType(BaseType));
        }
    }
}