using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class CapturedLocal : Expression
    {
        public Function Function;
        public Variable Variable;

        public CapturedLocal(Source src, Function func, Variable var)
            : base(src)
        {
            Function = func;
            Variable = var;
        }

        public override ExpressionType ExpressionType => ExpressionType.CapturedLocal;

        public override DataType ReturnType => Variable.ValueType;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("local::" + Variable.Name);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new CapturedLocal(Source, state.GetMember(Function), state.GetVariable(Variable, true));
        }
    }
}