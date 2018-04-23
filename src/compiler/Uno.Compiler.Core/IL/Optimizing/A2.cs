using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Optimizing
{
    class A2 : CompilerPass
    {
        public A2(CompilerPass parent)
            : base(parent)
        {
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            var call = e as CallExpression;
            if (call != null &&
                call.Function.MasterDefinition.Stats.HasFlag(EntityStats.ThrowsException))
                Function.Stats |= EntityStats.ThrowsException;
        }
    }
}