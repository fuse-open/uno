using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class This : Expression
    {
        public DataType ThisType;

        public override DataType ReturnType => ThisType;
        public override ExpressionType ExpressionType => ExpressionType.This;

        public This(Source src, DataType dt)
            : base(src)
        {
            ThisType = dt;
        }

        public This(DataType dt)
            : base(Source.Unknown)
        {
            ThisType = dt;
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("this");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new This(Source, state.GetType(ThisType));
        }
    }
}