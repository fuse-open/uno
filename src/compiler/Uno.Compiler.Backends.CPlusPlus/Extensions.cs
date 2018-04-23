using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.CPlusPlus
{
    public static class Extensions
    {
        public static bool CanUsePointerDirectly(this Field f)
        {
            return f.IsStatic && 
                !f.DeclaringType.HasInitializer &&
                f.DeclaringType.MasterDefinition.IsClosed;
        }

        public static bool HasStorage(this Expression e)
        {
            switch (e?.ExpressionType)
            {
                case ExpressionType.LoadElement:
                case ExpressionType.LoadField:
                case ExpressionType.LoadLocal:
                    return true;
                case ExpressionType.SequenceOp:
                    return ((SequenceOp) e).Right.HasStorage();
                default:
                    return (e as CallExpression)?.Storage != null;
            }
        }
    }
}