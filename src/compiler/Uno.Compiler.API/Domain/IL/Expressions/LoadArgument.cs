using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class LoadArgument : Expression
    {
        public Either<Function, Lambda> Function;
        public int Index;

        public LoadArgument(Source src, Either<Function, Lambda> func, int index)
            : base(src)
        {
            Index = index;
            Function = func;
        }

        public LoadArgument(Source src, Function func, int index)
            : this(src, new Either<Function, Lambda>(func), index)
        {
        }

        public LoadArgument(Source src, Lambda lam, int index)
            : this(src, new Either<Function, Lambda>(lam), index)
        {
        }

        public Parameter Parameter => Function.Match(
            f => f.Parameters[Index],
            l => l.Parameters[Index]);

        public override DataType ReturnType => Parameter.Type;

        public override ExpressionType ExpressionType => ExpressionType.LoadArgument;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append(Parameter.Name);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return Function.Match(
                fun => new LoadArgument(Source, state.Function, Index),
                lam => new LoadArgument(Source, state.GetLambda(lam), Index));
        }
    }
}