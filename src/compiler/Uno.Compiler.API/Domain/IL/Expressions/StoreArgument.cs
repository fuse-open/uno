using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class StoreArgument : Expression
    {
        public Either<Function, Lambda> Function;
        public int Index;
        public Expression Value;

        public StoreArgument(Source src, Either<Function, Lambda> func, int index, Expression value)
            : base(src)
        {
            Function = func;
            Index = index;
            Value = value;
        }

        public StoreArgument(Source src, Function func, int index, Expression value)
            : this(src, new Either<Function, Lambda>(func), index, value)
        {
        }

        public StoreArgument(Source src, Lambda lam, int index, Expression value)
            : this(src, new Either<Function, Lambda>(lam), index, value)
        {
        }

        public Parameter Parameter => Function.Match(
            f => f.Parameters[Index],
            l => l.Parameters[Index]);

        public override DataType ReturnType => Value.ReturnType;

        public override ExpressionType ExpressionType => ExpressionType.StoreArgument;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.AppendWhen(u >= ExpressionUsage.Operand, "(");
            sb.Append(Parameter.Name + " = ");
            Value.Disassemble(sb);
            sb.AppendWhen(u >= ExpressionUsage.Operand, ")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Value);
            Value.Visit(p);
            p.End(ref Value);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return Function.Match(
                fun => new StoreArgument(Source, state.Function, Index, Value.CopyExpression(state)),
                lam => new StoreArgument(Source, state.GetLambda(lam), Index, Value.CopyExpression(state)));
        }
    }
}