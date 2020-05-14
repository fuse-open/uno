using System;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Logging;

namespace Uno.Compiler.Core.IL.Optimizing
{
    class ConstantFolder : CompilerPass
    {
        public ConstantFolder(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Condition => !Backend.IsDefault;

        public override void End(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.CallBinOp:
                {
                    var s = e as CallBinOp;
                    var c = TryFoldOperator(s.Operator, s.Source, s.Left, s.Right);

                    if (c != null)
                        e = c;

                    break;
                }
                case ExpressionType.CallUnOp:
                {
                    var s = e as CallUnOp;
                    var c = TryFoldOperator(s.Operator, s.Source, s.Operand);

                    if (c != null)
                        e = c;

                    break;
                }
                case ExpressionType.CallCast:
                {
                    var s = e as CallCast;
                    var c = TryFoldCast(s.Cast, s.Source, s.Operand);

                    if (c != null)
                        e = c;

                    break;
                }
                case ExpressionType.BranchOp:
                {
                    var s = e as BranchOp;
                    var left = s.Left as Constant;
                    var right = s.Right as Constant;

                    if (left != null && right != null)
                    {
                        switch (s.BranchType)
                        {
                            case BranchType.Or:
                                e = new Constant(s.Source, s.ReturnType, (bool)left.Value || (bool)right.Value);
                                break;

                            case BranchType.And:
                                e = new Constant(s.Source, s.ReturnType, (bool)left.Value && (bool)right.Value);
                                break;
                        }
                    }

                    break;
                }
                case ExpressionType.ConditionalOp:
                {
                    var s = e as ConditionalOp;
                    var c = s.Condition as Constant;

                    if (c != null)
                        e = (bool)c.Value ? s.True : s.False;

                    break;
                }
                case ExpressionType.CastOp:
                {
                    var s = e as CastOp;
                    var c = s.Operand as Constant;

                    if (c != null && (c.Value == null ||
                            s.ReturnType.IsIntrinsic && s.ReturnType != Essentials.Object ||
                            s.ReturnType.IsEnum && s.ReturnType != Essentials.Object))
                        e = new Constant(s.Source, s.ReturnType, c.Value);
                    break;
                }
                case ExpressionType.LoadLocal:
                {
                    var s = e as LoadLocal;

                    if (s.Variable.IsConstant && s.Variable.OptionalValue != null)
                        e = s.Variable.OptionalValue;

                    break;
                }
                case ExpressionType.GetProperty:
                {
                    var s = e as GetProperty;
                    var fat = s.Property.DeclaringType as FixedArrayType;

                    if (fat != null && s.Property.UnoName == "Length" && fat.OptionalSize != null)
                        e = fat.OptionalSize;

                    break;
                }
            }
        }

        public override void End(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.IfElse:
                {
                    var s = e as IfElse;
                    var c = s.Condition as Constant;

                    if (c != null)
                    {
                        var value = (bool)c.Value;

                        if (value)
                        {
                            e = s.OptionalIfBody ?? new NoOp(s.Source);
                            Begin(ref e);
                        }
                        else
                        {
                            e = s.OptionalElseBody ?? new NoOp(s.Source);
                            Begin(ref e);
                        }
                    }

                    break;
                }
            }
        }

        public Constant TryMakeConstant(Expression e)
        {
            this.VisitNullable(ref e);
            return e as Constant;
        }

        public void TryMakeConstant(ref Expression e)
        {
            this.VisitNullable(ref e);
        }

        static object TryFoldCastChecked(Cast cast, dynamic v)
        {
            switch (cast.ReturnType.BuiltinType)
            {
                case BuiltinType.SByte:
                case BuiltinType.Byte:
                case BuiltinType.Short:
                case BuiltinType.UShort:
                case BuiltinType.Int:
                case BuiltinType.UInt:
                case BuiltinType.Long:
                case BuiltinType.ULong:
                    if (v is double || v is float)
                    {
                        var d = (double)v;
                        if (!double.IsInfinity(d) && !double.IsNaN(d))
                            v = (long)d;
                    }

                    break;
            }

            checked
            {
                switch (cast.ReturnType.BuiltinType)
                {
                    case BuiltinType.Char: return (char)v;
                    case BuiltinType.Float: return (float)v;
                    case BuiltinType.Double: return (double)v;
                    case BuiltinType.Byte: return (byte)v;
                    case BuiltinType.SByte: return (sbyte)v;
                    case BuiltinType.Short: return (short)v;
                    case BuiltinType.UShort: return (ushort)v;
                    case BuiltinType.UInt: return (uint)v;
                    case BuiltinType.Int: return (int)v;
                    case BuiltinType.ULong: return (ulong)v;
                    case BuiltinType.Long: return (long)v;
                }
            }

            return null;
        }

        static object TryFoldCastUnchecked(Cast cast, dynamic v)
        {
            switch (cast.ReturnType.BuiltinType)
            {
                case BuiltinType.Char: return (char)v;
                case BuiltinType.Float: return (float)v;
                case BuiltinType.Double: return (double)v;
                case BuiltinType.Byte: return (byte)v;
                case BuiltinType.SByte: return (sbyte)v;
                case BuiltinType.Short: return (short)v;
                case BuiltinType.UShort: return (ushort)v;
                case BuiltinType.UInt: return (uint)v;
                case BuiltinType.Int: return (int)v;
                case BuiltinType.ULong: return (ulong)v;
                case BuiltinType.Long: return (long)v;
            }

            return null;
        }

        public Expression TryFoldCast(Cast cast, Source src, Expression operand, bool checkResult, bool reportError, Log log, out bool overflow)
        {
            overflow = false;

            var c = operand as Constant;
            if (c == null) return null;

            if (checkResult)
            {
                try
                {
                    var result = TryFoldCastChecked(cast, c.Value);

                    if (result != null)
                        return new Constant(src, cast.ReturnType, result);
                }
                catch (Exception)
                {
                    overflow = true;

                    if (reportError)
                        Log.Error(src, ErrorCode.E0106, "Constant value " + c.Quote() + " cannot be converted to " + cast.ReturnType.Quote());

                    return new Constant(src, DataType.Invalid, -1);
                }
            }
            else
            {
                var result = TryFoldCastUnchecked(cast, c.Value);

                if (result != null)
                    return new Constant(src, cast.ReturnType, result);
            }

            return null;
        }

        public Expression TryFoldCast(Cast cast, Source src, Expression operand, bool checkResult, Log log)
        {
            bool overflow;
            return TryFoldCast(cast, src, operand, checkResult, true, log, out overflow);
        }

        public Expression TryFoldCast(Cast cast, Source src, Expression operand)
        {
            bool overflow;
            return TryFoldCast(cast, src, operand, false, false, null, out overflow);
        }

        public Expression TryFoldOperator(Operator op, Source src, Expression operand)
        {
            var c = operand as Constant;
            if (c == null) return null;

            try
            {
                dynamic v = c.Value;
                switch (op.Type)
                {
                    case OperatorType.UnaryPlus:
                        return GetConstant(src, op.ReturnType, +v);
                    case OperatorType.UnaryNegation:
                        return GetConstant(src, op.ReturnType, -v);
                    case OperatorType.LogicalNot:
                        return GetConstant(src, op.ReturnType, !v);
                    case OperatorType.OnesComplement:
                        return GetConstant(src, op.ReturnType, ~v);
                }
            }
            catch (Exception e)
            {
                Log.Trace(e);
                Log.Warning(op.Source, ErrorCode.I0000, e.Message);
            }

            return null;
        }

        public static bool IsZero(object v)
        {
            if (v is double)
            {
                var d = (double)v;
                return d == 0.0 || d == -0.0;
            }

            if (v is float)
            {
                var f = (float)v;
                return f == 0.0f || f == -0.0f;
            }

            if (v != null)
                return v.Equals(0);

            return false;
        }

        public static bool IsOne(object v)
        {
            if (v is double)
                return (double)v == 1.0;

            if (v is float)
                return (float)v == 1.0f;

            if (v != null)
                return v.Equals(1);

            return false;
        }

        public static bool IsNullOrEmpty(object v)
        {
            return v == null || v.Equals("");
        }

        public Expression TryFoldOperator(Operator op, Source src, Expression left, Expression right)
        {
            var c0 = left as Constant;
            var c1 = right as Constant;

            if (c0 != null && c1 != null)
            {
                if (op.ReturnType.BuiltinType == BuiltinType.String && op.Type == OperatorType.Addition && c0.Value == null && c1.Value == null)
                    return new Constant(src, op.ReturnType, ""); // 'null + null' == 'empty string'

                try
                {
                    dynamic v0 = c0.Value;
                    dynamic v1 = c1.Value;

                    switch (op.Type)
                    {
                        case OperatorType.Addition:
                            return GetConstant(src, op.ReturnType, v0 + v1);
                        case OperatorType.Subtraction:
                            return GetConstant(src, op.ReturnType, v0 - v1);
                        case OperatorType.Multiply:
                            return GetConstant(src, op.ReturnType, v0 * v1);
                        case OperatorType.Division:
                            return GetConstant(src, op.ReturnType, v0 / v1);
                        case OperatorType.Modulus:
                            return GetConstant(src, op.ReturnType, v0 % v1);
                        case OperatorType.BitwiseOr:
                            return GetConstant(src, op.ReturnType, v0 | v1);
                        case OperatorType.BitwiseAnd:
                            return GetConstant(src, op.ReturnType, v0 & v1);
                        case OperatorType.ExclusiveOr:
                            return GetConstant(src, op.ReturnType, v0 ^ v1);
                        case OperatorType.LeftShift:
                            return GetConstant(src, op.ReturnType, v0 << v1);
                        case OperatorType.RightShift:
                            return GetConstant(src, op.ReturnType, v0 >> v1);
                        case OperatorType.LessThan:
                            return GetConstant(src, op.ReturnType, v0 < v1);
                        case OperatorType.LessThanOrEqual:
                            return GetConstant(src, op.ReturnType, v0 <= v1);
                        case OperatorType.GreaterThan:
                            return GetConstant(src, op.ReturnType, v0 > v1);
                        case OperatorType.GreaterThanOrEqual:
                            return GetConstant(src, op.ReturnType, v0 >= v1);
                        case OperatorType.Equality:
                            return GetConstant(src, op.ReturnType, v0 == v1);
                        case OperatorType.Inequality:
                            return GetConstant(src, op.ReturnType, v0 != v1);
                    }
                }
                catch (Exception e)
                {
                    Log.Trace(e);
                    Log.Warning(op.Source, ErrorCode.I0000, e.Message);
                }
            }
            else if (c0 != null || c1 != null)
            {
                if (op.ReturnType.BuiltinType != BuiltinType.String)
                {
                    switch (op.Symbol)
                    {
                        case "+":
                            if (c0 != null)
                            {
                                if (IsZero(c0.Value))
                                    return right;
                            }
                            else
                            {
                                if (IsZero(c1.Value))
                                    return left;
                            }

                            break;

                        case "*":
                            if (c0 != null)
                            {
                                if (IsOne(c0.Value))
                                    return right;
                            }
                            else
                            {
                                if (IsOne(c1.Value))
                                    return left;
                            }

                            break;

                        // TODO:
                        // Handle cases such as '0 - x', '0 * x', '-1 * x', etc
                        // Remember to add sequence ops when result is '0/Inf/NaN' to avoid side effects

                        // TODO:
                        // Perhaps move this code to a dedicated optimizer module
                    }
                }
            }

            return null;
        }

        Constant GetConstant(Source src, DataType type, dynamic value)
        {
            var bt = type.IsEnum
                ? type.Base.BuiltinType
                : type.BuiltinType;
            switch (bt)
            {
                case BuiltinType.Byte:
                    return new Constant(src, type, (byte) value);
                case BuiltinType.SByte:
                    return new Constant(src, type, (sbyte) value);
                case BuiltinType.Short:
                    return new Constant(src, type, (short) value);
                case BuiltinType.UShort:
                    return new Constant(src, type, (ushort) value);
                case BuiltinType.Int:
                    return new Constant(src, type, (int) value);
                case BuiltinType.UInt:
                    return new Constant(src, type, (uint) value);
                case BuiltinType.Long:
                    return new Constant(src, type, (long) value);
                case BuiltinType.ULong:
                    return new Constant(src, type, (ulong) value);
                default:
                    return new Constant(src, type, value);
            }
        }
    }
}
