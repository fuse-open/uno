using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public bool IsArgumentListCompatible(Parameter[] list, params Expression[] args)
        {
            if (args.Length != list.Length)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                if (!IsParameterModifierCompatible(list[i].Modifier, args[i]))
                    return false;

                if (!list[i].Type.Equals(args[i].ReturnType))
                    return false;
            }

            return true;
        }

        public bool IsArgumentListCompatibleUsingDefaultValues(Parameter[] list, params Expression[] args)
        {
            if (args.Length != list.Length)
            {
                for (int i = args.Length; i < list.Length; i++)
                    if (list[i].OptionalDefault == null)
                        return false;

                if (args.Length > list.Length)
                    return false;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (!IsParameterModifierCompatible(list[i].Modifier, args[i]))
                    return false;

                if (!list[i].Type.Equals(args[i].ReturnType))
                    return false;
            }

            return true;
        }

        public bool IsArgumentListCompatibleUsingImplicitCasts(Parameter[] list, params Expression[] args)
        {
            if (args.Length != list.Length)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                if (!IsParameterModifierCompatible(list[i].Modifier, args[i]))
                    return false;

                if (list[i].IsReference && !list[i].Type.Equals(args[i].ReturnType))
                    return false;

                var sym = TryCompileImplicitCast(args[i].Source, list[i].Type, args[i]);
                if (sym == null) return false;
            }

            return true;
        }

        public static bool IsParameterModifierCompatible(ParameterModifier paramModifier, Expression arg)
        {
            switch (paramModifier)
            {
                case ParameterModifier.Out:
                    return arg is AddressOf && ((AddressOf)arg).AddressType == AddressType.Out;
                case ParameterModifier.Ref:
                    return arg is AddressOf && ((AddressOf)arg).AddressType == AddressType.Ref;
                case ParameterModifier.Const:
                    return arg is AddressOf && ((AddressOf)arg).AddressType <= AddressType.Const;
                default:
                    return !(arg is AddressOf);
            }
        }

        public Expression[] TryApplyDefaultValuesOnArgumentList(Source src, Parameter[] list, Expression[] args)
        {
            if (args.Length < list.Length)
            {
                var result = new Expression[list.Length];

                for (int i = args.Length; i < list.Length; i++)
                    if (list[i].OptionalDefault != null && list[i].Modifier == 0)
                        result[i] = GetParameterDefaultValue(src, list[i]);
                    else if (list[i].Modifier == ParameterModifier.Params && list[i].Type is RefArrayType)
                        result[i] = new NewArray(src, list[i].Type as RefArrayType, new Constant(src, Essentials.Int, 0));
                    else
                        return args;

                for (int i = 0; i < args.Length; i++)
                    result[i] = args[i];

                return result;
            }

            if (list.Length > 0 && list[list.Length - 1].Modifier == ParameterModifier.Params && list[list.Length - 1].Type is RefArrayType)
            {
                var arrType = list[list.Length - 1].Type as RefArrayType;

                if (args.Length == list.Length && args[list.Length - 1].ReturnType == arrType)
                    return args;

                var result = new Expression[list.Length];

                for (int i = 0; i < list.Length - 1; i++)
                    result[i] = args[i];

                if (args.Length == list.Length)
                {
                    var arr = TryCompileImplicitCast(src, arrType, args[list.Length - 1]);

                    if (arr != null)
                    {
                        result[list.Length - 1] = arr;
                        return result;
                    }
                }

                var paramArray = new Expression[args.Length - list.Length + 1];
                var elmType = arrType.ElementType;

                for (int i = 0; i < paramArray.Length; i++)
                {
                    var elm = TryCompileImplicitCast(src, elmType, args[i + list.Length - 1]);
                    if (elm == null) return args;

                    paramArray[i] = elm;
                }

                result[list.Length - 1] = new NewArray(src, arrType, paramArray);
                return result;
            }

            return args;
        }

        private Expression GetParameterDefaultValue(Source src, Parameter parameter)
        {
            if (parameter.Attributes.Length > 0)
            {
                switch (parameter.Type.BuiltinType)
                {
                    case BuiltinType.Int:
                        if (parameter.HasAttribute(Essentials.CallerLineNumberAttribute))
                            return new Constant(src, Essentials.Int, src.Line);
                        break;
                    case BuiltinType.String:
                        if (parameter.HasAttribute(Essentials.CallerFilePathAttribute))
                            return new Constant(src, Essentials.String, src.FullPath);
                        if (parameter.HasAttribute(Essentials.CallerMemberNameAttribute))
                            return new Constant(src, Essentials.String, Function.Name);
                        if (parameter.HasAttribute(Essentials.CallerPackageNameAttribute))
                            return new Constant(src, Essentials.String, src.Package.Name);
                        break;
                }
            }

            return parameter.OptionalDefault;
        }

        public Expression[] CompileArgumentList(IReadOnlyList<AstArgument> args)
        {
            var result = new Expression[args.Count];

            for (int i = 0; i < args.Count; i++)
            {
                result[i] = CompileExpression(args[i].Value);

                switch (args[i].Modifier)
                {
                    case 0:
                        break;
                    case ParameterModifier.Out:
                        result[i] = new AddressOf(result[i].ActualValue, AddressType.Out);
                        break;
                    case ParameterModifier.Ref:
                        result[i] = new AddressOf(result[i].ActualValue, AddressType.Ref);
                        break;
                    default:
                        Log.Error(args[i].Value.Source, ErrorCode.E0107, "Invalid argument modifier " + args[i].Modifier.ToLiteral().Quote());
                        break;
                }
            }

            return result;
        }

        List<T> FindCompatibleOverloads<T>(IReadOnlyList<T> candidates, params Expression[] args) where T : ParametersMember
        {
            var result = new List<T>();

            foreach (var c in candidates)
            {
                var m = c as Method;
                if (m != null && m.IsGenericDefinition)
                    continue;

                if (IsArgumentListCompatible(c.Parameters, args))
                    result.Add(c);
            }

            if (result.Count == 0)
            {
                foreach (var c in candidates)
                {
                    var m = c as Method;
                    if (m != null && m.IsGenericDefinition)
                        continue;

                    if (IsArgumentListCompatibleUsingImplicitCasts(c.Parameters, args))
                        result.Add(c);
                }
            }

            return result;
        }

        List<T> FindCompatibleOverloads<T>(Source src, IReadOnlyList<T> candidates, IReadOnlyList<AstArgument> args, out Expression[] resultArgs) where T : ParametersMember
        {
            var result = new List<T>();
            resultArgs = CompileArgumentList(args);

            var hasGenericMethodCandidates = false;

            foreach (var c in candidates)
            {
                var m = c as Method;

                if (m != null && m.IsGenericDefinition)
                {
                    hasGenericMethodCandidates = true;
                    continue;
                }

                if (IsArgumentListCompatible(c.Parameters, resultArgs))
                    result.Add(c);
            }

            if (result.Count == 0)
            {
                var candidateMethods = candidates as IReadOnlyList<Method>;

                if (hasGenericMethodCandidates && candidateMethods != null)
                {
                    candidates = CopyAndParameterizeGenericMethods(src, candidateMethods, resultArgs) as T[];

                    foreach (var c in candidates)
                    {
                        var m = c as Method;
                        if (m != null && m.IsGenericDefinition)
                            continue;

                        if (IsArgumentListCompatible(c.Parameters, resultArgs))
                            result.Add(c);
                    }

                    if (result.Count != 0)
                        return result;
                }

                foreach (var c in candidates)
                {
                    var m = c as Method;
                    if (m != null && m.IsGenericDefinition)
                        continue;

                    if (IsArgumentListCompatibleUsingDefaultValues(c.Parameters, resultArgs))
                        result.Add(c);
                }

                if (result.Count != 0)
                    return result;

                foreach (var c in candidates)
                {
                    var m = c as Method;
                    if (m != null && m.IsGenericDefinition)
                        continue;

                    var candidateArgs = TryApplyDefaultValuesOnArgumentList(src, c.Parameters, resultArgs);
                    if (IsArgumentListCompatibleUsingImplicitCasts(c.Parameters, candidateArgs))
                        result.Add(c);
                }
            }

            return result;
        }

        public void ApplyImplicitCastsOnArgumentList(Parameter[] list, Expression[] args)
        {
            if (list.Length != args.Length)
                throw new ArgumentException("Invalid number of arguments");

            for (int i = 0; i < args.Length; i++)
                args[i] = CompileImplicitCast(args[i].Source, list[i].Type, args[i]);
        }

        public bool TryApplyImplicitCastsOnArgumentList(Parameter[] list, Expression[] args)
        {
            if (!IsArgumentListCompatibleUsingImplicitCasts(list, args))
                return false;
            
            ApplyImplicitCastsOnArgumentList(list, args);
            return true;
        }

        static bool IsImplicitCastFromSignedToUnsignedIntegralType(DataType a, DataType b)
        {
            switch (a.BuiltinType)
            {
                case BuiltinType.SByte:
                    switch (b.BuiltinType)
                    {
                        case BuiltinType.Byte:
                        case BuiltinType.UShort:
                        case BuiltinType.UInt:
                        case BuiltinType.ULong:
                            return true;
                    }

                    return false;

                case BuiltinType.Short:
                    switch (b.BuiltinType)
                    {
                        case BuiltinType.UShort:
                        case BuiltinType.UInt:
                        case BuiltinType.ULong:
                            return true;
                    }

                    return false;

                case BuiltinType.Int:
                    switch (b.BuiltinType)
                    {
                        case BuiltinType.UInt:
                        case BuiltinType.ULong:
                            return true;
                    }

                    return false;

                case BuiltinType.Long:
                    return b.BuiltinType == BuiltinType.ULong;
            }

            return false;
        }

        public int CompareImplicitCasts(Expression e1, Expression e2)
        {
            if (!e1.ReturnType.Equals(e2.ReturnType))
            {
                var e12 = TryCompileImplicitCast(e1.Source, e2.ReturnType, new Default(e1.Source, e1.ReturnType));
                var e21 = TryCompileImplicitCast(e2.Source, e1.ReturnType, new Default(e2.Source, e2.ReturnType));

                if (e12 != null && e21 == null ||
                    IsImplicitCastFromSignedToUnsignedIntegralType(e1.ReturnType, e2.ReturnType))
                    return -1;

                if (e21 != null && e12 == null ||
                    IsImplicitCastFromSignedToUnsignedIntegralType(e2.ReturnType, e1.ReturnType))
                    return 1;

                if (e2.IsInvalid)
                    return -1;
                if (e1.IsInvalid)
                    return 1;
            }

            return 0;
        }

        int CountGenericArgumentsRecursive(DataType dt)
        {
            var c = 0;
            if (dt.IsGenericParameterization)
                for (var i = 0; i < dt.GenericArguments.Length; i++)
                    c += CountGenericArgumentsRecursive(dt.GenericArguments[i]) + 1;
            return c;
        }

        int CompareArgumentLists(Expression[] args, Expression[] result1, Expression[] result2)
        {
            var result = 0;

            for (int i = 0; i < args.Length; i++)
            {
                var diff = CompareImplicitCasts(
                    result1[i] ?? Expression.Invalid,
                    result2[i] ?? Expression.Invalid);

                if (diff != 0)
                {
                    if (result == 0)
                        result = diff;
                    else if (result != diff)
                        return 0;
                }
            }

            return result;
        }

        bool TryResolveAmbiguousMatch<T>(List<T> functions, params Expression[] args) where T : ParametersMember
        {
            for (int i = 0; i < functions.Count - 1;)
            {
                var f1 = new Expression[args.Length];
                var f2 = new Expression[args.Length];
                var p1 = functions[i + 0].Parameters;
                var p2 = functions[i + 1].Parameters;
                var l1 = Math.Min(p1.Length, args.Length);
                var l2 = Math.Min(p2.Length, args.Length);

                for (int j = 0; j < l1; j++)
                    f1[j] = TryCompileImplicitCast(args[j].Source, p1[j].Type, args[j]) ?? Expression.Invalid;
                for (int j = 0; j < l2; j++)
                    f2[j] = TryCompileImplicitCast(args[j].Source, p2[j].Type, args[j]) ?? Expression.Invalid;

                switch (CompareArgumentLists(args, f1, f2))
                {
                    case -1:
                        functions.RemoveAt(i + 1);
                        break;
                    case 1:
                        functions.RemoveRange(0, i + 1);
                        i = 0;
                        break;
                    default:
                        var m1 = functions[i + 0] as Method;
                        var m2 = functions[i + 1] as Method;

                        if (m1 != null && m2 != null)
                        {
                            var c1 = m1.IsGenericMethod 
                                ? CountGenericArgumentsRecursive(m1.GenericType) 
                                : 0;
                            var c2 = m2.IsGenericMethod
                                ? CountGenericArgumentsRecursive(m2.GenericType) 
                                : 0;

                            if (c1 == c2)
                                i++;
                            else if (c1 < c2)
                                functions.RemoveAt(i + 1);
                            else
                            {
                                functions.RemoveRange(0, i + 1);
                                i = 0;
                            }
                            break;
                        }

                        i++;
                        break;
                }
            }

            return functions.Count == 1;
        }

        public Expression ReportAmbiguousMatchError<T>(Source src, List<T> candidates, params Expression[] args) where T : ParametersMember
        {
            foreach (var a in args)
                if (a.IsInvalid)
                    return Expression.Invalid;

            return Error(src, ErrorCode.E0109, "Ambiguous match" + NameResolver.SuggestCandidates(candidates));
        }

        DataType TryGetGenericTypeArgumentRecursive(GenericParameterType gt, DataType pt, DataType at)
        {
            if (pt.Equals(gt))
                return at;

            if (at.IsArray)
            {
                if (pt.IsArray)
                    return TryGetGenericTypeArgumentRecursive(gt, pt.ElementType, at.ElementType);
                if (pt.MasterDefinition == Essentials.IEnumerable_T && pt.IsGenericParameterization)
                    return TryGetGenericTypeArgumentRecursive(gt, pt.GenericArguments[0], at.ElementType);
            }

            if (pt.IsFlattenedParameterization && at.IsFlattenedParameterization && pt.FlattenedArguments.Length == at.FlattenedArguments.Length)
            {
                for (int i = 0; i < pt.FlattenedArguments.Length; i++)
                {
                    var dt = TryGetGenericTypeArgumentRecursive(gt, pt.FlattenedArguments[i], at.FlattenedArguments[i]);
                    if (dt != null) return dt;
                }
            }

            for (int i = 0; i < at.Interfaces.Length; i++)
            {
                var dt = TryGetGenericTypeArgumentRecursive(gt, pt, at.Interfaces[i]);
                if (dt != null) return dt;
            }

            if (at.Base != null)
                return TryGetGenericTypeArgumentRecursive(gt, pt, at.Base);

            return null;
        }

        DataType TryGetGenericTypeFromMethodGroup(GenericParameterType gt, DelegateType dt, MethodGroup mg, GenericParameterType[] gparams, DataType[] gargs)
        {
            dt.AssignBaseType();
            var pl = dt.Parameters;

            foreach (var c in mg.Candidates)
            {
                var m = c;
                if (m.IsGenericDefinition)
                {
                    DataType[] typeArgs;
                    if (TryGetGenericTypeArgumentsFromParameterList(c.GenericParameters, c.Parameters,
                        dt.GenericArguments, out typeArgs))
                    {
                        for (int i = 0; i < typeArgs.Length; i++)
                        {
                            typeArgs[i] = TryParameterize(typeArgs[i], gparams, gargs);
                            if (typeArgs[i] == null)
                                goto CONTINUE;
                        }
                        
                        m = TypeBuilder.Parameterize(mg.Source, c, typeArgs);
                    }
                    else
                        goto CONTINUE;
                }

                for (int j = 0; j < Math.Min(pl.Length, m.Parameters.Length); j++)
                {
                    var at = TryGetGenericTypeArgumentRecursive(gt, pl[j].Type, m.Parameters[j].Type);
                    if (at != null) return at;
                }

                if (!m.ReturnType.IsVoid)
                {
                    var at = TryGetGenericTypeArgumentRecursive(gt, dt.ReturnType, m.ReturnType);
                    if (at != null) return at;
                }
            CONTINUE:
                ; 
            }

            return null;
        }

        DataType TryParameterize(DataType dt, GenericParameterType[] gparams, DataType[] gargs)
        {
            if (dt.IsClosed)
                return dt;

            if (dt.IsGenericParameter)
                for (int i = 0; i < gparams.Length; i++)
                    if (dt.Equals(gparams[i]))
                        return gargs[i];

            return null;
        }

        bool TryGetGenericTypeArgumentsFromArgumentList(GenericParameterType[] gt, Parameter[] pl, Expression[] args, out DataType[] result)
        {
            result = new DataType[gt.Length];

            for (int i = 0; i < gt.Length; i++)
            {
                for (int j = 0; j < Math.Min(pl.Length, args.Length); j++)
                {
                    result[i] = args[j].ExpressionType == ExpressionType.MethodGroup && pl[j].Type.IsDelegate
                        ? TryGetGenericTypeFromMethodGroup(gt[i], (DelegateType) pl[j].Type, (MethodGroup) args[j], gt, result)
                        : TryGetGenericTypeArgumentRecursive(gt[i], pl[j].Type, args[j].ReturnType);
                    if (result[i] != null)
                        goto FOUND;
                }

                return false;
            FOUND:
                ;
            }

            return true;
        }

        bool TryGetGenericTypeArgumentsFromParameterList(GenericParameterType[] gt, Parameter[] pl, DataType[] args, out DataType[] result)
        {
            result = new DataType[gt.Length];

            for (int i = 0; i < gt.Length; i++)
            {
                for (int j = 0; j < Math.Min(pl.Length, args.Length); j++)
                {
                    result[i] = TryGetGenericTypeArgumentRecursive(gt[i], pl[j].Type, args[j]);
                    if (result[i] != null)
                        goto FOUND;
                }

                return false;
            FOUND:
                ;
            }

            return true;
        }

        public Method[] CopyAndParameterizeGenericMethods(Source src, IReadOnlyList<Method> candidates, Expression[] args)
        {
            var result = new Method[candidates.Count];

            for (int i = 0; i < result.Length; i++)
            {
                var c = result[i] = candidates[i];

                if (c.IsGenericDefinition)
                {
                    DataType[] typeArgs;
                    if (TryGetGenericTypeArgumentsFromArgumentList(c.GenericParameters, c.Parameters, args, out typeArgs))
                        result[i] = TypeBuilder.Parameterize(src, c, typeArgs);
                }
            }

            return result;
        }

        public bool TryResolveMethodOverload(Source src, IReadOnlyList<Method> candidates, IReadOnlyList<AstArgument> args, out Method method, out Expression[] resultArgs)
        {
            var result = FindCompatibleOverloads(src, candidates, args, out resultArgs);

            if (result.Count == 0)
            {
                method = null;
                return false;
            }

            if (result.Count != 1 && !TryResolveAmbiguousMatch(result, resultArgs))
                ReportAmbiguousMatchError(src, result, resultArgs);

            method = result[0];
            resultArgs = TryApplyDefaultValuesOnArgumentList(src, method.Parameters, resultArgs);

            ApplyImplicitCastsOnArgumentList(method.Parameters, resultArgs);
            return true;
        }

        public bool TryResolveConstructorOverload(Source src, IReadOnlyList<Constructor> candidates, IReadOnlyList<AstArgument> args, out Constructor ctor, out Expression[] resultArgs)
        {
            var result = FindCompatibleOverloads(src, candidates, args, out resultArgs);

            if (result.Count == 0)
            {
                ctor = null;
                return false;
            }

            if (result.Count != 1 && !TryResolveAmbiguousMatch(result, resultArgs))
                ReportAmbiguousMatchError(src, result, resultArgs);

            ctor = result[0];
            resultArgs = TryApplyDefaultValuesOnArgumentList(src, ctor.Parameters, resultArgs);

            ApplyImplicitCastsOnArgumentList(ctor.Parameters, resultArgs);
            return true;
        }

        public bool TryResolveIndexerOverload(Source src, IReadOnlyList<Property> candidates, IReadOnlyList<AstArgument> args, out Property indexer, out Expression[] resultArgs)
        {
            var result = FindCompatibleOverloads(src, candidates, args, out resultArgs);

            if (result.Count == 0)
            {
                indexer = null;
                return false;
            }

            if (result.Count != 1 && !TryResolveAmbiguousMatch(result, resultArgs))
                ReportAmbiguousMatchError(src, result, resultArgs);

            indexer = result[0];
            resultArgs = TryApplyDefaultValuesOnArgumentList(src, indexer.Parameters, resultArgs);

            ApplyImplicitCastsOnArgumentList(indexer.Parameters, resultArgs);
            return true;
        }

        public Cast TryResolveCastOverload(Source src, IReadOnlyList<Cast> candidates, ref Expression arg, bool reportError, out bool wasAmbiguous)
        {
            wasAmbiguous = false;
            var result = FindCompatibleOverloads(candidates, arg);

            if (result.Count == 0)
                return null;

            if (result.Count != 1 && !TryResolveAmbiguousMatch(result, arg))
            {
                wasAmbiguous = true;

                if (reportError)
                    ReportAmbiguousMatchError(src, result, arg);
            }

            arg = CompileImplicitCast(src, result[0].Parameters[0].Type, arg);
            return result[0];
        }

        public Cast TryResolveCastOverload(Source src, IReadOnlyList<Cast> candidates, ref Expression arg)
        {
            bool wasAmigouos;
            return TryResolveCastOverload(src, candidates, ref arg, true, out wasAmigouos);
        }

        public Operator TryResolveOperatorOverload(Source src, IReadOnlyList<Operator> candidates, Expression[] args)
        {
            var result = FindCompatibleOverloads(candidates, args);

            if (result.Count == 0)
                return null;

            if (result.Count != 1 && !TryResolveAmbiguousMatch(result, args))
                ReportAmbiguousMatchError(src, result, args);

            ApplyImplicitCastsOnArgumentList(result[0].Parameters, args);
            return result[0];
        }

        public Constructor TryResolveConstructorOverload(Source src, IReadOnlyList<Constructor> candidates, Expression[] args)
        {
            var result = FindCompatibleOverloads(candidates, args);

            if (result.Count == 0)
                return null;

            if (result.Count != 1 && !TryResolveAmbiguousMatch(result, args))
                ReportAmbiguousMatchError(src, result, args);

            ApplyImplicitCastsOnArgumentList(result[0].Parameters, args);
            return result[0];
        }
    }
}

