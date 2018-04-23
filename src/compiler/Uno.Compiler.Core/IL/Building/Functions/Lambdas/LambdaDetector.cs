using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Building.Functions.Lambdas
{
    class LambdaDetector : CompilerPass
    {
        public bool Result;

        public LambdaDetector(CompilerPass parent)
            : base(parent)
        {
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.Lambda:
                {
                    Result = true;
                    break;
                }
                case ExpressionType.UncompiledLambda:
                {
                    Backend.Log.Error(e.Source, ErrorCode.E0000, "Unresolved lambda expression");
                    break;
                }
            }
        }
    }
}