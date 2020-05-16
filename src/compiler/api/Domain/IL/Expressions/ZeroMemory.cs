using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class ZeroMemory : VoidExpression
    {
        public Variable Variable;

        public ZeroMemory(Source src, Variable variable)
            : base(src)
        {
            Variable = variable;
        }

        public override ExpressionType ExpressionType => ExpressionType.ZeroMemory;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("zero_memory(&" + Variable.Name + ")");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new ZeroMemory(Source, state.GetVariable(Variable));
        }
    }
}