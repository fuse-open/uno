using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public class PartialParameter : PartialExpression
    {
        public readonly int Index;
        public readonly Either<Function, Lambda> Function;

        public PartialParameter(Source src, Lambda lambda, int index)
            : base(src)
        {
            Index = index;
            Function = new Either<Function, Lambda>(lambda);
        }

        public PartialParameter(Source src, Function function, int index)
            : base(src)
        {
            Index = index;
            Function = new Either<Function, Lambda>(function);
        }

        public Parameter Parameter => Function.Match(
            fun => fun.Parameters[Index],
            lam => lam.Parameters[Index]);

        public override PartialExpressionType ExpressionType => PartialExpressionType.Parameter;

        public override string ToString()
        {
            return Parameter.Name;
        }
    }
}