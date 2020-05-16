using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver
    {
        public DataType GetType(PartialExpression p, AstExpression e)
        {
            if (p.IsInvalid)
                return DataType.Invalid;

            if (p.ExpressionType == PartialExpressionType.Type)
            {
                var dt = (p as PartialType).Type;

                if (!dt.IsGenericDefinition || e is AstGeneric)
                    return dt;

                Log.Error(e.Source, ErrorCode.E3131, "Expression is a generic type definition but is used as a type.");
                return DataType.Invalid;
            }

            Log.Error(e.Source, ErrorCode.E3132, "Expression is <" + p.ExpressionType + "> but is used as a type.");
            return DataType.Invalid;
        }

        public DataType GetType(Namescope scope, AstExpression e)
        {
            return GetType(ResolveExpression(scope, e, null), e);
        }
    }
}
