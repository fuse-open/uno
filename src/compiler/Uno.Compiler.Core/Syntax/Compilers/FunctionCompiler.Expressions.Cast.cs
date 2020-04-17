using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public Expression CompileCast(AstCast ce)
        {
            var targetType = NameResolver.GetType(Namescope, ce.TargetType);
            var operand = CompileExpression(ce.Argument);
            return TryCompileCast(ce.Source, targetType, operand) ??
                Error(ce.Source, ErrorCode.E2029, operand.ReturnType.Quote() + " has no defined cast to type " + targetType.Quote());
        }

        Expression TryCompileCast(Source src, DataType targetType, Expression operand)
        {
            if (targetType.IsInvalid || operand.IsInvalid)
                return Expression.Invalid;

            // Equal types == NOP
            if (targetType.Equals(operand.ReturnType))
                return operand;

            var cast = TryResolveCastOverload(src, NameResolver.GetTypeCasts(targetType, operand.ReturnType), ref operand);
            if (cast != null)
                return new CallCast(src, cast, operand);

            // Box
            // Generic -> object
            // Method -> delegate
            // etc
            var impl = TryCompileImplicitCast(src, targetType, operand);
            if (impl != null)
                return impl;

            // Down cast
            if (targetType.IsReferenceType && operand.ReturnType.IsReferenceType || 
                    targetType.IsInterface || operand.ReturnType.IsInterface)
                return operand.ReturnType.IsInterface ||
                        operand.ReturnType.IsSubclassOf(targetType) ||
                        operand.ReturnType.IsImplementingInterface(targetType) ||
                        targetType.IsInterface ||
                        targetType.IsSubclassOf(operand.ReturnType) ||
                        targetType.IsImplementingInterface(operand.ReturnType)
                    ? new CastOp(src, targetType, operand)
                    : Error(src, ErrorCode.E2028, targetType.Quote() + " and " + operand.ReturnType.Quote() + " are not compatible types");

            // Unbox
            // Ref -> generic
            if (targetType.IsValueType && operand.ReturnType.Equals(Essentials.Object) ||
                targetType.IsGenericParameter && operand.ReturnType.IsReferenceType)
                return new CastOp(src, targetType, operand);

            if (operand.ReturnType.IsEnum)
            {
                operand.ReturnType.AssignBaseType();
                return TryCompileCast(src, targetType, new CastOp(src, operand.ReturnType.Base ?? Essentials.Int, operand));
            }

            if (targetType.IsEnum)
            {
                targetType.AssignBaseType();
                var intValue = TryCompileCast(src, targetType.Base ?? Essentials.Int, operand);
                if (intValue != null)
                    return new CastOp(src, targetType, intValue);
            }

            return null;
        }

        public Expression CompileImplicitCast(Source src, string expectedType, Expression value)
        {
            var result = TryCompileImplicitCast(src, ILFactory.GetType(src, expectedType), value, true);
            ImplicitCastStack.RemoveLast();
            return result;
        }

        public Expression CompileImplicitCast(Source src, DataType expectedType, Expression value)
        {
            var result = TryCompileImplicitCast(src, expectedType, value, true);
            ImplicitCastStack.RemoveLast();
            return result;
        }

        public Expression TryCompileImplicitCast(Source src, DataType expectedType, Expression value)
        {
            var result = TryCompileImplicitCast(src, expectedType, value, false);
            ImplicitCastStack.RemoveLast();
            return result;
        }

        Expression TryCompileImplicitCast(Source src, DataType expectedType, Expression value, bool reportErrorIfNotFound)
        {
            if (ImplicitCastStack.Contains(expectedType))
                ImplicitCastStack.Add(expectedType);
            else
            {
                ImplicitCastStack.Add(expectedType);

                if (Equals(expectedType, value.ReturnType))
                    return value;

                if (value.ReturnType is InvalidType || expectedType is InvalidType)
                    return Expression.Invalid;

                // (EnumType)0
                if (expectedType is EnumType && value is Constant && value.ConstantValue is int && (int)value.ConstantValue == 0)
                    return new Constant(value.Source, (EnumType)expectedType, (int)value.ConstantValue);

                // Null type (must be done before delegate)
                if (value.ReturnType.IsNull)
                    return expectedType.IsReferenceType
                            ? new Constant(src, expectedType, null) :
                        !reportErrorIfNotFound
                            ? null
                            : Error(src, ErrorCode.E2043, "'<null>' has no implicit cast to " + expectedType.Quote() + " because that is not a reference type");

                // Delegate
                if (expectedType is DelegateType && value is MethodGroup)
                {
                    var dt = expectedType as DelegateType;
                    dt.AssignBaseType();

                    var mg = value as MethodGroup;
                    var candidates = mg.Candidates;
                    var compatibleMethods = new List<Method>();

                    foreach (var m in candidates)
                        if (!m.IsGenericDefinition &&
                            m.ReturnType.Equals(dt.ReturnType) &&
                            dt.CompareParameters(m))
                            compatibleMethods.Add(m);

                    if (compatibleMethods.Count == 0)
                    {
                        foreach (var c in candidates)
                        {
                            if (c.IsGenericDefinition)
                            {
                                var dummyArgs = new Expression[dt.Parameters.Length];

                                for (int i = 0; i < dummyArgs.Length; i++)
                                {
                                    var p = dt.Parameters[i];
                                    dummyArgs[i] = new Default(p.Source, p.Type);

                                    switch (p.Modifier)
                                    {
                                        case ParameterModifier.Const:
                                            dummyArgs[i] = new AddressOf(dummyArgs[i], AddressType.Const);
                                            break;
                                        case ParameterModifier.Ref:
                                            dummyArgs[i] = new AddressOf(dummyArgs[i], AddressType.Ref);
                                            break;
                                        case ParameterModifier.Out:
                                            dummyArgs[i] = new AddressOf(dummyArgs[i], AddressType.Out);
                                            break;
                                    }
                                }

                                candidates = CopyAndParameterizeGenericMethods(src, candidates, dummyArgs);

                                foreach (var m in candidates)
                                    if (!m.IsGenericDefinition &&
                                        m.ReturnType.Equals(dt.ReturnType) &&
                                        dt.CompareParameters(m))
                                        compatibleMethods.Add(m);
                                break;
                            }
                        }

                        if (compatibleMethods.Count == 0)
                            foreach (var m in candidates)
                                if (!m.IsGenericDefinition &&
                                    (m.ReturnType.Equals(dt.ReturnType) || m.ReturnType.IsReferenceType && m.ReturnType.IsSubclassOfOrEqual(dt.ReturnType)) &&
                                    dt.CompareParametersEqualOrSubclassOf(m))
                                    compatibleMethods.Add(m);
                    }

                    if (compatibleMethods.Count == 1)
                    {
                        var m = compatibleMethods[0];

                        if (m.IsStatic)
                            return new NewDelegate(src, dt, null, m);

                        if (mg.Object != null && !mg.Object.ReturnType.IsReferenceType)
                            mg.Object = new CastOp(src, Essentials.Object, mg.Object.ActualValue);

                        return new NewDelegate(src, dt, mg.Object, m);
                    }

                    return !reportErrorIfNotFound
                        ? null
                        : compatibleMethods.Count == 0
                            ? Error(src, ErrorCode.E2045, "No methods matches the parameter list and return type of delegate type " + dt.Quote())
                            : ReportAmbiguousMatchError(src, compatibleMethods);
                }

                // Lambda
                if (expectedType is DelegateType && value is UncompiledLambda)
                {
                    expectedType.AssignBaseType();
                    return TryCompileImplicitLambdaCast((UncompiledLambda) value, (DelegateType) expectedType);
                }

                // Constant
                if (value is Constant && expectedType.IsIntrinsic)
                {
                    var constant = value as Constant;

                    if (constant.Value is int)
                    {
                        int intValue = (int)constant.Value;

                        switch (expectedType.BuiltinType)
                        {
                            case BuiltinType.SByte:
                                if (intValue >= -0x80 && intValue <= 0x7f)
                                    return new Constant(constant.Source, expectedType, (sbyte)intValue);
                                break;
                            case BuiltinType.Byte:
                                if (intValue >= 0 && intValue <= 0xff)
                                    return new Constant(constant.Source, expectedType, (byte)intValue);
                                break;
                            case BuiltinType.Short:
                                if (intValue >= -0x8000 && intValue <= 0x7fff)
                                    return new Constant(constant.Source, expectedType, (short)intValue);
                                break;
                            case BuiltinType.UShort:
                                if (intValue >= 0 && intValue <= 0xffff)
                                    return new Constant(constant.Source, expectedType, (ushort)intValue);
                                break;
                            case BuiltinType.UInt:
                                if (intValue >= 0)
                                    return new Constant(constant.Source, expectedType, (uint)intValue);
                                break;
                            case BuiltinType.ULong:
                                if (intValue >= 0)
                                    return new Constant(constant.Source, expectedType, (ulong)intValue);
                                break;
                        }
                    }

                    if (constant.Value is long)
                    {
                        long longValue = (long)constant.Value;

                        switch (expectedType.BuiltinType)
                        {
                            case BuiltinType.ULong:
                                if (longValue >= 0)
                                    return new Constant(constant.Source, expectedType, (ulong)longValue);
                                break;
                        }
                    }
                }

                // Implict cast
                bool ambiguous;
                var cast = TryResolveCastOverload(src, NameResolver.GetTypeCasts(expectedType, value.ReturnType),
                    ref value, reportErrorIfNotFound, out ambiguous);

                if (ambiguous)
                    goto ERROR;

                if (cast != null && cast.IsImplicitCast)
                    return new CallCast(src, cast, value);

                // Up cast
                if (expectedType == Essentials.Object ||
                    value.ReturnType.IsSubclassOf(expectedType) ||
                    expectedType.IsInterface && value.ReturnType.IsImplementingInterface(expectedType))
                    return new CastOp(src, expectedType, value);

                // Fixed Array
                if (expectedType.IsFixedArray && value.ReturnType.IsFixedArray)
                {
                    var t1 = expectedType as FixedArrayType;
                    var t2 = value.ReturnType as FixedArrayType;

                    if (t1.ElementType.Equals(t2.ElementType) && (
                            t1.OptionalSize == null || t2.OptionalSize == null ||
                            t1.OptionalSize is Constant && t2.OptionalSize is Constant && Equals(t1.OptionalSize.ConstantValue, t2.OptionalSize.ConstantValue)
                        ))
                        return value;
                }

                // T[] -> IEnumerable<T>
                if (value.ReturnType.IsRefArray && expectedType.MasterDefinition == Essentials.IEnumerable_T)
                {
                    ImplicitCastStack.RemoveLast();
                    var arrayEnumerable = TypeBuilder.Parameterize(src, Essentials.ArrayEnumerable_T, value.ReturnType.ElementType);
                    return TryCompileImplicitCast(src, expectedType,
                        ILFactory.NewObject(src, arrayEnumerable, value), reportErrorIfNotFound);
                }
            }

        ERROR:
            return !reportErrorIfNotFound
                ? null
                : Error(src, ErrorCode.E2047, "No implicit cast from " + value.ReturnType.Quote() + " to " + expectedType.Quote());
        }
    }
}
