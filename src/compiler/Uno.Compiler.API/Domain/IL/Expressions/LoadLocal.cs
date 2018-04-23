using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class LoadLocal : Expression
    {
        public Variable Variable;

        public override DataType ReturnType => Variable.ValueType;

        public LoadLocal(Source src, Variable var)
            : base(src)
        {
            Variable = var;
        }

        public override ExpressionType ExpressionType => ExpressionType.LoadLocal;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append(Variable.Name);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new LoadLocal(Source, state.GetVariable(Variable));
        }
    }
}