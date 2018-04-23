using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class PlaceholderArgument : Expression
    {
        readonly DataType _dt;
        public int Index;

        public PlaceholderArgument(Source src, DataType dt, int index)
            : base(src)
        {
            _dt = dt;
            Index = index;
        }

        public override ExpressionType ExpressionType => ExpressionType.PlaceholderArgument;

        public override DataType ReturnType => _dt;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("meta[" + Index + "]");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new PlaceholderArgument(Source, state.GetType(_dt), Index);
        }
    }
}