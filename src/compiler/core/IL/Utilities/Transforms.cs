using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Logging;

namespace Uno.Compiler.Core.IL.Utilities
{
    static class Transforms
    {
        // TODO: The indirection stuff is not good and will be replaced when implementing non-nullable references
        public static bool NeedsIndirection(this Expression e)
        {
            if (e == null)
                return false;

            switch (e.ExpressionType)
            {
                case ExpressionType.Base:
                case ExpressionType.This:
                case ExpressionType.Constant:
                case ExpressionType.Default:
                case ExpressionType.LoadLocal:
                case ExpressionType.LoadArgument:
                case ExpressionType.RuntimeConst:
                case ExpressionType.LoadVarying:
                case ExpressionType.LoadVertexAttrib:
                case ExpressionType.LoadPixelSampler:
                case ExpressionType.LoadUniform:
                    return false;

                case ExpressionType.AddressOf:
                    return (e as AddressOf).Operand.NeedsIndirection();

                default:
                    return true;
            }
        }

        public static bool TryCreateIndirection(Namescope scope, ref Expression e, out Expression s)
        {
            if (e.NeedsIndirection())
            {
                var v = new Variable(e.Source, null, scope.GetUniqueIdentifier("ind"), e.ReturnType, VariableType.Indirection);
                var a = e as AddressOf;
                var q = e as SequenceOp;

                if (q?.Right is AddressOf)
                {
                    a = q.Right as AddressOf;
                    q.Right = a.ActualValue;
                }

                s = new StoreLocal(e.Source, v, e.ActualValue);
                e = new LoadLocal(e.Source, v);

                if (a != null)
                    e = new AddressOf(e, a.AddressType);

                return true;
            }

            s = null;
            return false;
        }

        public static Expression TryCreateIndirection(Namescope scope, ref Expression e)
        {
            Expression ind;
            TryCreateIndirection(scope, ref e, out ind);
            return ind;
        }

        public static bool TryCreateReadonlyValueFieldIndirection(Namescope scope, ref Expression e)
        {
            var address = e as AddressOf;

            var lf = address?.Operand as LoadField;

            if (lf == null)
                return false;

            var f = lf.Field;
            var dt = f.ReturnType;

            if (f.FieldModifiers.HasFlag(FieldModifiers.ReadOnly) && dt.IsValueType)
            {
                Expression ind;
                if (TryCreateIndirection(scope, ref e, out ind))
                {
                    e = new SequenceOp(ind, e);
                    return true;
                }
            }

            return false;
        }

        public static bool TryTransformEnumBinOpToIntBinOp(this CallBinOp s, Log log, ref Expression result)
        {
            // Enum.BinOps -> Int.BinOps
            if (s.Operator.DeclaringType is EnumType)
            {
                var bt = s.Operator.DeclaringType.Base;

                foreach (var o in bt.Operators)
                {
                    if (o.UnoName == s.Operator.UnoName)
                    {
                        result = new CallBinOp(s.Source, o,
                                               new CastOp(s.Source, bt, s.Left),
                                               new CastOp(s.Source, bt, s.Right));

                        if (s.ReturnType == s.Operator.DeclaringType)
                            result = new CastOp(s.Source, s.Operator.DeclaringType, result);

                        return true;
                    }
                }

                log.Error(s.Source, ErrorCode.I0072, "'" + bt + "." + s.Operator.UnoName + "(" + bt + "," + bt + ")' was not found");
            }

            return false;
        }

        public static bool TryTransformEnumUnOpToIntUnOp(this CallUnOp s, Log log, ref Expression result)
        {
            // Enum.UnOps -> Int.UnOps
            if (s.Operator.DeclaringType is EnumType)
            {
                var bt = s.Operator.DeclaringType.Base;

                foreach (var o in bt.Operators)
                {
                    if (o.UnoName == s.Operator.UnoName)
                    {
                        result = new CallUnOp(s.Source, o,
                                              new CastOp(s.Source, bt, s.Operand));

                        if (s.ReturnType == s.Operator.DeclaringType)
                            result = new CastOp(s.Source, s.Operator.DeclaringType, result);

                        return true;
                    }
                }

                log.Error(s.Source, ErrorCode.I0071, "'" + bt + "." + s.Operator.UnoName + "(" + bt + ")' was not found");
            }

            return false;
        }

        public static bool TryTransformEnumHasFlagToIntOps(this CallMethod s, Log log, ref Expression result)
        {
            // Enum.HasFlag() -> (((int)a & (int)b) == (int)b)
            if (s.Method.DeclaringType is EnumType && s.Method.UnoName == "HasFlag" && s.Arguments.Length == 1)
            {
                var bt = s.Method.DeclaringType.Base;

                foreach (var o in bt.Operators)
                {
                    if (o.UnoName == "op_BitwiseAnd")
                    {
                        var a = new CastOp(s.Source, bt, s.Object.ActualValue);
                        var b = new CastOp(s.Source, bt, s.Arguments[0]);

                        foreach (var p in bt.Operators)
                        {
                            if (p.UnoName == "op_Equality")
                            {
                                result = new CallBinOp(s.Source, p, new CallBinOp(s.Source, o, a, b), b);
                                return true;
                            }
                        }

                        log.Error(s.Source, ErrorCode.I0069, "'" + bt + ".op_Equality(" + bt + "," + bt + ")' was not found");
                        return false;
                    }
                }

                log.Error(s.Source, ErrorCode.I0069, "'" + bt + ".op_BitwiseAnd(" + bt + "," + bt + ")' was not found");
            }

            return false;
        }

        public static bool TryTransformDelegateBinOp(this CallBinOp s, ILFactory ilf, ref Expression result)
        {
            DataType dt = s.Operator.DeclaringType;
            if (dt is DelegateType)
            {
                if (s.Operator.Symbol == "+")
                {
                    result = new CastOp(s.Source, dt, ilf.CallMethod(s.Source, dt, "Combine", s.Left, s.Right));
                    return true;
                }
                if (s.Operator.Symbol == "-")
                {
                    result = new CastOp(s.Source, dt, ilf.CallMethod(s.Source, dt, "Remove", s.Left, s.Right));
                    return true;
                }
            }

            return false;
        }

        public static Expression TransformSwizzleToNewObject(this Swizzle s, Namescope scope)
        {
            // a.XYZ -> (temp = a, new T(temp.X, temp.Y, temp.Z))
            //   OR     new T(a.X, a.Y, a.Z)

            var obj = s.Object;
            var ind = TryCreateIndirection(scope, ref obj);

            var args = new Expression[s.Fields.Length];
            for (int i = 0; i < s.Fields.Length; i++)
                args[i] = new LoadField(s.Source, obj, s.Fields[i]);

            var result = new NewObject(s.Source, s.Constructor, args);
            return ind != null
                ? (Expression) new SequenceOp(ind, result)
                :              result;
        }

        public static Expression TransformNullOpToConditionalOp(this NullOp s, Essentials types, Namescope scope)
        {
            // a ?? b -> (temp = a, temp != null ? temp : b)
            //   OR   -> a != null ? a : b

            var left = s.Left;
            var right = s.Right;
            var ind = TryCreateIndirection(scope, ref left);

            var cond = new ReferenceOp(s.Source, types.Bool, EqualityType.NotEqual, left, new Constant(s.Source, left.ReturnType, null));
            var result = new ConditionalOp(s.Source, cond, left, right);

            return ind != null
                ? (Expression) new SequenceOp(ind, result)
                : result;
        }

        // TODO: Get rid of parent parameter
        public static bool TryTransformSetPropertyChainToSequence(this SetProperty s, Namescope scope, Statement parent, ref Expression result)
        {
            if (parent == null ||
                parent is SequenceOp && (parent as SequenceOp).Left == s)
                return false;

            var ind = TryCreateIndirection(scope, ref s.Value);

            result = new SequenceOp(s, s.Value);

            if (ind != null)
                result = new SequenceOp(ind, result);

            return true;
        }

        public static bool TryTransformGetFixedArrayLength(this GetProperty s, Log log, ref Expression result)
        {
            var fat = s.Property.DeclaringType as FixedArrayType;

            if (fat != null && s.Property.UnoName == "Length")
            {
                if (fat.OptionalSize != null)
                    result = fat.OptionalSize;
                else
                {
                    log.Error(s.Source, ErrorCode.E0000, "Cannot get length of 'fixed' array with unknown size");
                    result = new InvalidExpression();
                }

                return true;
            }

            return false;
        }
    }
}
