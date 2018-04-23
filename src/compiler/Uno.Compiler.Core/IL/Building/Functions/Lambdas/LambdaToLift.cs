using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Building.Functions.Lambdas
{
    struct LambdaToLift
    {
        public readonly Lambda Lambda;
        public readonly Variables FreeVars;

        public LambdaToLift(Lambda lambda, Variables freeVars)
        {
            Lambda = lambda;
            FreeVars = freeVars;
        }
    }
}