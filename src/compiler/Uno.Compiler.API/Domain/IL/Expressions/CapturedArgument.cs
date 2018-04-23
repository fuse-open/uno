using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class CapturedArgument : Expression
    {
        public Function Function;
        public int ParameterIndex;

        public CapturedArgument(Source src, Function func, int index)
            : base(src)
        {
            Function = func;
            ParameterIndex = index;
        }

        public override ExpressionType ExpressionType => ExpressionType.CapturedArgument;

        public override DataType ReturnType => Function.Parameters[ParameterIndex].Type;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("local::" + Function.Parameters[ParameterIndex].Name);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new CapturedArgument(Source, state.GetMember(Function), ParameterIndex);
        }
    }
}