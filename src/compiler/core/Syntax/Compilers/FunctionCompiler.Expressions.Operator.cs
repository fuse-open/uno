using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.Syntax.Binding;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Expression CompileUnOp(AstUnary uo)
        {
            var operand = CompileExpression(uo.Operand);
            if (operand.IsInvalid)
                return Expression.Invalid;

            switch (uo.Type)
            {
                case AstUnaryType.IncreasePrefix:
                case AstUnaryType.DecreasePrefix:
                case AstUnaryType.IncreasePostfix:
                case AstUnaryType.DecreasePostfix:
                    return !operand.ReturnType.IsIntegralType && !operand.ReturnType.IsFloatingPointType
                        ? Error(uo.Source, ErrorCode.E2063, "Incremental operators can only be used on integers")
                        : new FixOp(uo.Source, (FixOpType) uo.Type, operand);
            }

            var args = new[] {operand};
            var opOp = uo.Type.ToSymbol();
            var op = TryResolveOperatorOverload(uo.Source, NameResolver.GetTypeOperators(operand.ReturnType, opOp), args);
            return op != null
                ? new CallUnOp(uo.Source, op, args[0])
                : Error(uo.Source, ErrorCode.E2065, operand.ReturnType.Quote() + " has no operators matching the argument list");
        }

        public Expression CompileBinOp(AstBinary bo)
        {
            return bo.IsAssign
                ? CompileAssign(bo)
                : CompileBinOp(bo.Source, bo.Type, 
                        CompileExpression(bo.Left), 
                        CompileExpression(bo.Right));
        }

        Expression CompileAssign(AstBinary e)
        {
            var lval = ResolveExpression(e.Left, null);
            if (lval.IsInvalid)
                return Expression.Invalid;

            var right = CompileExpression(e.Right);

            // Incremental assign-operators (+=, -= etc.)
            if (e.Type != AstBinaryType.Assign)
            {
                // Special case for +=, -= on events
                if (lval.ExpressionType == PartialExpressionType.Event)
                {
                    var p = lval as PartialEvent;
                    var obj = p.Object;

                    if (obj != null)
                        Transforms.TryCreateReadonlyValueFieldIndirection(Namescope, ref obj);

                    switch (e.Type)
                    {
                        case AstBinaryType.AddAssign:
                            return new AddListener(e.Source, obj, p.Event, CompileImplicitCast(e.Source, p.Event.ReturnType, right));
                        case AstBinaryType.SubAssign:
                            return new RemoveListener(e.Source, obj, p.Event, CompileImplicitCast(e.Source, p.Event.ReturnType, right));
                    }
                }

                right = CompileBinOp(e.Source, e.RemoveAssign, CompilePartial(lval), right);
            }

            switch (lval.ExpressionType)
            {
                case PartialExpressionType.ArrayElement:
                {
                    var p = lval as PartialArrayElement;
                    return new StoreElement(e.Source, p.Object, p.Index, CompileImplicitCast(right.Source, p.ElementType, right));
                }
                case PartialExpressionType.Variable:
                {
                    var p = lval as PartialVariable;
                    return new StoreLocal(e.Source, p.Variable, CompileImplicitCast(right.Source, p.Variable.ValueType, right));
                }
                case PartialExpressionType.Parameter:
                {
                    var p = lval as PartialParameter;

                    return new StoreArgument(e.Source, p.Function, p.Index, CompileImplicitCast(right.Source, p.Parameter.Type, right));
                }
                case PartialExpressionType.Field:
                {
                    var p = lval as PartialField;
                    return new StoreField(e.Source, VerifyLValue(p.Object) ? p.Object : Expression.Invalid, p.Field, CompileImplicitCast(right.Source, p.Field.ReturnType, right));
                }
                case PartialExpressionType.Property:
                {
                    var p = lval as PartialProperty;
                    var s = CompileImplicitCast(right.Source, p.Property.ReturnType, right);
                    return p.Property.SetMethod == null
                        ? Error(lval.Source, ErrorCode.E2070, "The property " + p.Property.Quote() + " has no setter accessor")
                        : new SetProperty(e.Source, p.Object, p.Property, s);
                }
                case PartialExpressionType.Event:
                {
                    var p = lval as PartialEvent;
                    var s = CompileImplicitCast(right.Source, p.Event.ReturnType, right);
                    return p.Event.ImplicitField == null
                        ? Error(lval.Source, ErrorCode.E2071, "The event " + p.Event.Quote() + " has no implicit field and can only be used before '+=' or '-='")
                        : new StoreField(e.Source, p.Object, p.Event.ImplicitField, s);
                }
                case PartialExpressionType.Indexer:
                {
                    var p = lval as PartialIndexer;
                    var s = CompileImplicitCast(right.Source, p.Indexer.ReturnType, right);
                    return p.Indexer.SetMethod == null
                        ? Error(lval.Source, ErrorCode.E2026, "The indexer " + p.Indexer.Quote() + " has no setter accessor")
                        : new SetProperty(e.Source, p.Object, p.Indexer, s, p.Arguments);
                }
                case PartialExpressionType.This:
                {
                    return !IsFunctionScope
                        ? Error(lval.Source, ErrorCode.E0000, "'this' not allowed in current context")
                        : new StoreThis(e.Source, CompileImplicitCast(right.Source, Function.DeclaringType, right));
                }
            }

            return Error(e.Left.Source, ErrorCode.E2027, "Expression of type <" + lval.ExpressionType + "> cannot be used as left-hand side in an assignment");
        }

        Expression CompileBinOp(Source src, AstBinaryType binOp, Expression left, Expression right)
        {
            if (left.IsInvalid || right.IsInvalid)
                return Expression.Invalid;

            if ((left.ReturnType is NullType || left.ReturnType is MethodGroupType) && right.ReturnType.IsReferenceType)
            {
                var leftCast = TryCompileImplicitCast(src, right.ReturnType, left);
                if (leftCast != null)
                    left = leftCast;
            }
            else if ((right.ReturnType is NullType || right.ReturnType is MethodGroupType) && left.ReturnType.IsReferenceType)
            {
                var rightCast = TryCompileImplicitCast(src, left.ReturnType, right);
                if (rightCast != null)
                    right = rightCast;
            }

            var args = new[] {left, right};
            var opOp = binOp.ToSymbol();
            var op = TryResolveOperatorOverload(src, 
                NameResolver.GetTypeOperators(left.ReturnType, right.ReturnType, opOp), 
                args);

            if (op != null)
                return new CallBinOp(src, op, args[0], args[1]);

            switch (binOp)
            {
                case AstBinaryType.LogAnd:
                case AstBinaryType.LogOr:
                {
                    left = CompileImplicitCast(src, Essentials.Bool, left);
                    right = CompileImplicitCast(src, Essentials.Bool, right);
                    return new BranchOp(
                        src, Essentials.Bool, 
                        binOp == AstBinaryType.LogAnd 
                            ? BranchType.And 
                            : BranchType.Or, 
                        left, right);
                }
                case AstBinaryType.Null:
                {
                    if (left.ReturnType.IsNull)
                        return right;
                    if (!left.ReturnType.IsReferenceType)
                        return Error(src, ErrorCode.E2015, "'??' cannot be used on operand of type " + left.ReturnType.Quote() + " because it is not a reference type");

                    right = CompileImplicitCast(src, left.ReturnType, right);
                    return new NullOp(src, left, right);
                }
                case AstBinaryType.Equal:
                case AstBinaryType.NotEqual:
                {
                    if (left.ReturnType.IsReferenceType && right.ReturnType.IsReferenceType)
                        return new ReferenceOp(src, Essentials.Bool, binOp == AstBinaryType.Equal ? EqualityType.Equal : EqualityType.NotEqual, left, right);
                    break;
                }
                case AstBinaryType.Sequence:
                    return new SequenceOp(left, right);
            }

            return Error(src, ErrorCode.E2016, left.ReturnType.Quote() + " has no operators " + binOp.ToSymbol().Quote() + " matching the argument list");
        }
    }
}
