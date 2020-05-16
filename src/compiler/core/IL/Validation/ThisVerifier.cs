using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Validation
{
    class ThisVerifier : Pass
    {
        public ThisVerifier(CompilerPass parent)
            : base(parent)
        {
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            if (e.ExpressionType == ExpressionType.This)
                Log.Error(e.Source, ErrorCode.E4111, "'this' is not available in current context");
            else if (e.ExpressionType == ExpressionType.Base)
                Log.Error(e.Source, ErrorCode.E0000, "'base' is not available in current context");
        }
    }
}