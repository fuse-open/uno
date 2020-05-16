using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.Syntax.Generators
{
    static class Expressions
    {
        public static bool IsVariableOrGlobal(Expression e)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.AddressOf:
                    return IsVariableOrGlobal(e.ActualValue);

                case ExpressionType.LoadElement:
                {
                    var s = e as LoadElement;
                    return IsVariableOrGlobal(s.Array) && IsVariableOrGlobal(s.Index);
                }
                case ExpressionType.LoadField:
                {
                    var s = e as LoadField;
                    return s.Object == null || IsVariableOrGlobal(s.Object);
                }
                case ExpressionType.Swizzle:
                {
                    var s = e as Swizzle;
                    return s.Object == null || IsVariableOrGlobal(s.Object);
                }

                case ExpressionType.LoadLocal:
                case ExpressionType.LoadArgument:
                case ExpressionType.LoadPixelSampler:
                case ExpressionType.LoadUniform:
                case ExpressionType.LoadVarying:
                case ExpressionType.LoadVertexAttrib:
                case ExpressionType.RuntimeConst:
                case ExpressionType.This:
                case ExpressionType.Base:
                case ExpressionType.Constant:
                case ExpressionType.ExternOp:
                case ExpressionType.ExternString:
                    return true;

                default:
                    return false;
            }
        }
    }
}
