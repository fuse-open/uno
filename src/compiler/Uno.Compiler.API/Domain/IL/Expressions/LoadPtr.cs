using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class LoadPtr : Expression
    {
        public Expression Argument;

        public LoadPtr(Expression arg)
            : base(arg.Source)
        {
            Argument = arg;
        }

        public override DataType ReturnType => Argument.ReturnType;

        public override ExpressionType ExpressionType => ExpressionType.LoadPtr;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("load_ptr(");
            Argument.Disassemble(sb);
            sb.Append(")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.VisitNullable(ref Argument, u);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new LoadPtr(Argument.CopyExpression(state));
        }
    }
}
