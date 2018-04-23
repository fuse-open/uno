using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Compiler.Core.Syntax.Compilers;
using Uno.Compiler.Frontend.Analysis;
using Uno.Logging;

namespace Uno.Compiler.Core.IL.Utilities
{
    public class ILFactory : LogObject, IILFactory
    {
        readonly Backend _backend;
        readonly Namespace _il;
        readonly NameResolver _resolver;
        readonly Compiler _compiler;

        public readonly Essentials Essentials;
        IEssentials IILFactory.Essentials => Essentials;

        internal ILFactory(Backend backend, Namespace il, Essentials essentials, NameResolver resolver, Compiler compiler)
            : base(compiler)
        {
            _backend = backend;
            _il = il;
            Essentials = essentials;
            _resolver = resolver;
            _compiler = compiler;
        }

        public SourcePackage TryGetPackage(string packageName)
        {
            foreach (var package in _compiler.Input.Packages)
                if (package.Name == packageName)
                    return package;

            return null;
        }

        public DataType GetType(string type)
        {
            return GetType(Source.Unknown, type);
        }

        public DataType GetType(Source src, string type)
        {
            return _resolver.GetType(_il, Parser.ParseType(Log, src, type));
        }

        public IEntity GetEntity(string str, params Namescope[] scopes)
        {
            return GetEntity(Source.Unknown, str, scopes);
        }

        public IEntity GetEntity(Source src, string str, params Namescope[] scopes)
        {
            if (string.IsNullOrEmpty(str))
            {
                Log.Error(src, ErrorCode.E0000, "<null> could not be resolved");
                return DataType.Invalid;
            }

            // Special case void because that is not a valid expression
            if (str == "void")
                return DataType.Void;

            var log = new Log(TextWriter.Null);
            var e = str[0] == '.' ? null : Parser.ParseExpression(log, src, str);

            if (log.HasErrors || e == null)
            {
                // Custom code to handle special cases not supported in the parser.
                // E.g.: foo(ref fixed float[8]), .ctor(), .cctor()

                string basePart = null;
                AstArgument[] args = null;

                str = str.Trim();

                if (str.EndsWith(')'))
                {
                    var parts = str.Split('(');

                    if (parts.Length == 2)
                    {
                        basePart = parts[0];
                        var argsPart = parts[1].Substring(0, parts[1].Length - 1);
                        if (argsPart.Length > 0)
                        {
                            var argParts = argsPart.Split(',');
                            args = new AstArgument[argParts.Length];

                            for (int i = 0; i < args.Length; i++)
                            {
                                var argPart = argParts[i].Trim();

                                if (argPart.StartsWith("ref fixed ", StringComparison.InvariantCulture) && 
                                    argPart.EndsWith(']'))
                                {
                                    argPart = argPart.Substring(10);

                                    var index = argPart.LastIndexOf('[');
                                    var sizePart = argPart.Substring(index + 1, argPart.Length - index - 2);

                                    AstExpression size = null;
                                    if (sizePart.Length > 0)
                                        size = Parser.ParseExpression(Log, src, sizePart);

                                    var type = Parser.ParseType(Log, src, argPart.Substring(0, index));
                                    args[i] = new AstArgument(null, ParameterModifier.Ref, new AstFixedArray(src, type, size));
                                }
                                else if (argPart.StartsWith("ref ", StringComparison.InvariantCulture))
                                    args[i] = new AstArgument(null, ParameterModifier.Ref, Parser.ParseType(Log, src, argPart.Substring(4)));
                                else if (argPart.StartsWith("out ", StringComparison.InvariantCulture))
                                    args[i] = new AstArgument(null, ParameterModifier.Out, Parser.ParseType(Log, src, argPart.Substring(4)));
                                else
                                    args[i] = Parser.ParseType(Log, src, argPart);
                            }
                        }
                    }
                }

                if (basePart == null)
                    basePart = str;

                if (basePart == ".ctor" || basePart == ".cctor")
                    e = new AstIdentifier(src, basePart);
                else
                    e = Parser.ParseExpression(Log, src, basePart);

                if (args != null)
                    e = new AstCall(AstCallType.Function, e, args);
                else if (basePart != str)
                    e = new AstCall(AstCallType.Function, e);
            }

            var result = ResolveExpression(e, null, scopes ?? new Namescope[0]);

            if (result is Function && e.ExpressionType != AstExpressionType.Call)
            {
                Log.Error(src, ErrorCode.E3355, str.Quote() + " is a method and must include the parameter list");
                return DataType.Invalid;
            }

            if (result is IEntity)
                return result as IEntity;

            Log.Error(src, ErrorCode.E3356, str.Quote() + " could not be resolved");
            return DataType.Invalid;
        }

        public Expression GetExpression(string expression, DataType expectedType = null)
        {
            return GetExpression(Source.Unknown, expression, expectedType);
        }

        public Expression GetExpression(string expression, string expectedType)
        {
            return GetExpression(Source.Unknown, expression, expectedType);
        }

        public Expression GetExpression(Source src, string expression, DataType expectedType = null)
        {
            return _compiler.CompileExpression(Parser.ParseExpression(Log, src, expression), _il, expectedType);
        }

        public Expression GetExpression(Source src, string expression, string expectedType)
        {
            return GetExpression(src, expression, GetType(src, expectedType));
        }

        public Expression NewObject(string type, params Expression[] args)
        {
            return NewObject(Source.Unknown, type, args);
        }

        public Expression NewObject(DataType type, params Expression[] args)
        {
            return NewObject(Source.Unknown, type, args);
        }

        public Expression NewObject(Source src, DataType type, params Expression[] args)
        {
            bool found = false;
            var fc = new FunctionCompiler(_compiler, _il);

            type.PopulateMembers();

            foreach (var m in type.Constructors)
            {
                if (m.IsPublic)
                {
                    if (fc.IsArgumentListCompatible(m.Parameters, args))
                        return new NewObject(src, m, args);

                    found = true;
                }
            }

            if (found)
            {
                foreach (var m in type.Constructors)
                {
                    if (m.IsPublic)
                    {
                        var processedArgs = fc.TryApplyDefaultValuesOnArgumentList(src, m.Parameters, args);

                        if (fc.IsArgumentListCompatibleUsingImplicitCasts(m.Parameters, processedArgs))
                        {
                            fc.ApplyImplicitCastsOnArgumentList(m.Parameters, processedArgs);
                            return new NewObject(src, m, processedArgs);
                        }
                    }
                }
            }

            Log.Error(src, ErrorCode.E0000, "Constructor not found: '" + type + "..ctor'");
            return Expression.Invalid;
        }

        public Expression NewObject(Source src, string type, params Expression[] args)
        {
            return NewObject(src, GetType(src, type), args);
        }

        public Expression CallMethod(string type, string method, params Expression[] args)
        {
            return CallMethod(Source.Unknown, type, method, args);
        }

        public Expression CallMethod(DataType type, string method, params Expression[] args)
        {
            return CallMethod(Source.Unknown, type, method, args);
        }

        public Expression CallMethod(Expression obj, string method, params Expression[] args)
        {
            return CallMethod(Source.Unknown, obj, method, args);
        }

        public Expression CallMethod(Source src, DataType dt, Expression obj, string method, params Expression[] args)
        {
            var fc = new FunctionCompiler(_compiler, dt, obj);
            var mg = fc.CompileExpression(Parser.ParseExpression(Log, src, method));
            if (mg.IsInvalid) return mg;

            if (mg is MethodGroup)
            {
                foreach (var m in (mg as MethodGroup).Candidates)
                    if (fc.IsArgumentListCompatible(m.Parameters, args))
                        return new CallMethod(src, obj, m, args);

                foreach (var m in (mg as MethodGroup).Candidates)
                {
                    var processedArgs = fc.TryApplyDefaultValuesOnArgumentList(src, m.Parameters, args);

                    if (fc.IsArgumentListCompatibleUsingImplicitCasts(m.Parameters, processedArgs))
                    {
                        fc.ApplyImplicitCastsOnArgumentList(m.Parameters, processedArgs);
                        return new CallMethod(src, obj, m, processedArgs);
                    }
                }
            }

            var pl = "(";
            var first = true;
            foreach (var a in args)
            {
                if (!first) pl += ",";
                pl += a.ReturnType;
                first = false;
            }
            pl += ")";

            Log.Error(src, ErrorCode.E0000, "No matching method overload: '" + dt + "." + method + pl + "'");
            return Expression.Invalid;
        }

        public Expression CallMethod(Source src, Expression obj, string method, params Expression[] args)
        {
            return CallMethod(src, obj.ReturnType, obj, method, args);
        }

        public Expression CallMethod(Source src, DataType type, string method, params Expression[] args)
        {
            return CallMethod(src, type, null, method, args);
        }

        public Expression CallMethod(Source src, string type, string method, params Expression[] args)
        {
            return CallMethod(src, GetType(src, type), null, method, args);
        }

        public Expression CallOperator(string type, string op, params Expression[] args)
        {
            return CallOperator(Source.Unknown, type, op, args);
        }

        public Expression CallOperator(DataType type, string op, params Expression[] args)
        {
            return CallOperator(Source.Unknown, type, op, args);
        }

        public Expression CallOperator(Source src, DataType dt, string op, params Expression[] args)
        {
            var fc = new FunctionCompiler(_compiler, dt, null);
            var type = Operator.Parse(args.Length, op);

            foreach (var m in dt.Operators)
                if (m.Type == type && fc.IsArgumentListCompatible(m.Parameters, args))
                    switch (args.Length)
                    {
                        case 1:
                            return new CallUnOp(src, m, args[0]);
                        case 2:
                            return new CallBinOp(src, m, args[0], args[1]);
                    }

            foreach (var m in dt.Operators)
            {
                if (m.Type == type)
                {
                    if (fc.IsArgumentListCompatibleUsingImplicitCasts(m.Parameters, args))
                    {
                        fc.ApplyImplicitCastsOnArgumentList(m.Parameters, args);

                        switch (args.Length)
                        {
                            case 1:
                                return new CallUnOp(src, m, args[0]);
                            case 2:
                                return new CallBinOp(src, m, args[0], args[1]);
                        }
                    }
                }
            }

            var pl = "(";
            var first = true;
            foreach (var a in args)
            {
                if (!first) pl += ",";
                pl += a.ReturnType;
                first = false;
            }
            pl += ")";

            Log.Error(src, ErrorCode.E0000, "No matching operator overload: '" + dt + "." + op + pl + "'");
            return Expression.Invalid;
        }

        public Expression CallOperator(Source src, string type, string op, params Expression[] args)
        {
            return CallOperator(src, GetType(src, type), op, args);
        }

        public Expression GetProperty(string type, string property)
        {
            return GetProperty(Source.Unknown, type, property);
        }

        public Expression GetProperty(DataType type, string property)
        {
            return GetProperty(Source.Unknown, type, property);
        }

        public Expression GetProperty(Expression obj, string property)
        {
            return GetProperty(Source.Unknown, obj, property);
        }

        public Expression GetProperty(Source src, DataType dt, Expression obj, string property)
        {
            var prop = dt.TryGetProperty(property, obj != null);
            if (prop != null)
                return new GetProperty(src, obj != null && prop.DeclaringType != dt ? new CastOp(obj.Source, prop.DeclaringType, obj) : obj, prop);

            Log.Error(src, ErrorCode.E0000, "No matching property: '" + dt + "." + property + ".get'");
            return Expression.Invalid;
        }

        public Expression GetProperty(Source src, Expression obj, string property)
        {
            return GetProperty(src, obj.ReturnType, obj, property);
        }

        public Expression GetProperty(Source src, DataType type, string property)
        {
            return GetProperty(src, type, null, property);
        }

        public Expression GetProperty(Source src, string type, string property)
        {
            return GetProperty(src, GetType(src, type), null, property);
        }

        public Expression SetProperty(Source src, DataType dt, Expression obj, string property, Expression value)
        {
            var fc = new FunctionCompiler(_compiler, dt, obj);

            foreach (var m in dt.Properties)
                if (m.UnoName == property && m.SetMethod != null)
                    return new SetProperty(src, obj, m, fc.CompileImplicitCast(src, m.ReturnType, value));

            Log.Error(src, ErrorCode.E0000, "No matching property: '" + dt + "." + property + ".set'");
            return Expression.Invalid;
        }

        public Expression SetProperty(string type, string property, Expression value)
        {
            return SetProperty(Source.Unknown, type, property, value);
        }

        public Expression SetProperty(DataType type, string property, Expression value)
        {
            return SetProperty(Source.Unknown, type, property, value);
        }

        public Expression SetProperty(Expression obj, string property, Expression value)
        {
            return SetProperty(Source.Unknown, obj, property, value);
        }

        public Expression SetProperty(Source src, Expression obj, string property, Expression value)
        {
            return SetProperty(src, obj.ReturnType, obj, property, value);
        }

        public Expression SetProperty(Source src, DataType type, string property, Expression value)
        {
            return SetProperty(src, type, null, property, value);
        }

        public Expression SetProperty(Source src, string type, string property, Expression value)
        {
            return SetProperty(src, GetType(src, type), null, property, value);
        }

        static DataType TryResolveInnerType(Source src, List<DataType> types, string name, AstExpression parent)
        {
            foreach (var t in types)
            {
                if (!t.IsAccessibleFrom(src) ||
                    t.IsGenericDefinition && !(parent is AstParameterizer || parent is AstGeneric) ||
                    !t.IsGenericDefinition && (parent is AstParameterizer || parent is AstGeneric))
                    continue;

                if (t.UnoName == name)
                    return t;
            }

            return null;
        }

        static Entity TryResolveEntity(Source src, Namescope root, string name, AstExpression parent)
        {
            if (root is Namespace)
            {
                var ns = root as Namespace;

                foreach (var cns in ns.Namespaces)
                    if (cns.UnoName == name)
                        return cns;

                foreach (var cns in ns.StrippedNamespaces)
                    if (cns.UnoName == name)
                        return cns;

                var it = TryResolveInnerType(src, ns.Types, name, parent) ?? 
                    TryResolveInnerType(src, ns.StrippedTypes, name, parent);
                if (it != null)
                    return it;
            }

            if (root is DataType)
            {
                var dt = root as DataType;
                dt.PopulateMembers();

                if (name == ".ctor")
                    return dt.Constructors.FirstOrDefault();

                if (name == ".cctor")
                    return dt.Initializer;

                var it = TryResolveInnerType(src, dt.NestedTypes, name, parent) ?? 
                    TryResolveInnerType(src, dt.StrippedTypes, name, parent);
                if (it != null)
                    return it;

                // Generic type parameters
                if (dt.IsGenericDefinition)
                    foreach (var gt in dt.GenericParameters)
                        if (gt.Name == name)
                            return gt;

                if (dt.Stats.HasFlag(EntityStats.ParameterizedDefinition))
                    foreach (var gt in dt.GenericArguments)
                        if (gt.UnoName == name)
                            return gt;

                // Static members
                foreach (var f in dt.Literals)
                    if (f.UnoName == name)
                        return f;

                foreach (var f in dt.Operators)
                    if (f.UnoName == name)
                        return f;

                var ct = dt;
                while (ct != null)
                {
                    foreach (var f in ct.Fields)
                        if (f.UnoName == name)
                            return f;

                    foreach (var f in ct.Methods)
                    {
                        if (f.UnoName == name)
                        {
                            if (f.IsGenericDefinition && !(parent is AstParameterizer || parent is AstGeneric) ||
                                !f.IsGenericDefinition && (parent is AstParameterizer || parent is AstGeneric))
                                continue;

                            return f;
                        }
                    }

                    foreach (var f in ct.Properties)
                    {
                        if (f.UnoName == name)
                            return f;

                        if (f.ImplicitField != null && f.ImplicitField.UnoName == name)
                            return f.ImplicitField;

                        if (f.GetMethod != null && f.GetMethod.UnoName == name)
                            return f.GetMethod;

                        if (f.SetMethod != null && f.SetMethod.UnoName == name)
                            return f.SetMethod;
                    }

                    foreach (var f in ct.Events)
                    {
                        if (f.UnoName == name)
                            return f;

                        if (f.AddMethod != null && f.AddMethod.UnoName == name)
                            return f.AddMethod;

                        if (f.RemoveMethod != null && f.RemoveMethod.UnoName == name)
                            return f.RemoveMethod;
                    }

                    foreach (var f in ct.StrippedMembers)
                    {
                        if (f.UnoName == name)
                        {
                            var m = f as Method;
                            if (m != null)
                            {
                                if (m.IsGenericDefinition && !(parent is AstParameterizer || parent is AstGeneric) ||
                                    !m.IsGenericDefinition && (parent is AstParameterizer || parent is AstGeneric))
                                    continue;
                            }

                            return f;
                        }
                    }

                    ct = ct.Base;
                }
            }

            return null;
        }

        static bool TypesEquals(DataType a, DataType b)
        {
            if (a == b)
                return true;

            if (a != null && b != null)
                if (a.Equals(b) ||
                    b.Prototype != b && a.Equals(b.Prototype) ||
                    a.Prototype != a && (a.Prototype.Equals(b) || a.Prototype.MasterDefinition.Equals(b.MasterDefinition))) // Workaround for bug in nested transform
                    return true;

            return false;
        }

        static bool TryMatchParameterList(Function f, IReadOnlyList<AstArgument> args, SourceObject[] types)
        {
            if (f.Parameters.Length != args.Count)
                return false;

            for (int i = 0; i < args.Count; i++)
            {
                if (!TypesEquals(f.Parameters[i].Type, types[i] as DataType))
                    return false;

                if (f.Parameters[i].IsReference && f.Parameters[i].Modifier != args[i].Modifier)
                    return false;
            }

            return true;
        }

        public Method Parameterize(Source src, Method definition, params DataType[] args)
        {
            return _compiler.TypeBuilder.Parameterize(src, definition, args);
        }

        public DataType Parameterize(Source src, DataType definition, params DataType[] args)
        {
            return _compiler.TypeBuilder.Parameterize(src, definition, args);
        }

        SourceObject ResolveExpression(AstExpression e, AstExpression p, params Namescope[] scopes)
        {
            switch (e.ExpressionType)
            {
                case AstExpressionType.BuiltinType:
                {
                    return Essentials.BuiltinTypes[(int) ((AstBuiltinType) e).BuiltinType];
                }
                case AstExpressionType.FixedArray:
                {
                    var s = e as AstFixedArray;
                    var root = ResolveExpression(s.ElementType, e, scopes);
                    Expression size = null;

                    if (s.OptionalSize != null)
                        size = _compiler.CompileExpression(s.OptionalSize, _il, Essentials.Int);

                    if (root is DataType)
                        return new FixedArrayType(e.Source, root as DataType, size, Essentials.Int);

                    break;
                }
                case AstExpressionType.Array:
                {
                    var s = e as AstUnary;
                    var root = ResolveExpression(s.Operand, e, scopes);

                    if (root is DataType)
                        return _compiler.TypeBuilder.GetArray(root as DataType);

                    break;
                }
                case AstExpressionType.Generic:
                {
                    var s = e as AstGeneric;
                    var root = ResolveExpression(s.Base, e, scopes);

                    if (root is DataType)
                    {
                        var dt = root as DataType;

                        if (dt.IsGenericDefinition && dt.GenericParameters.Length == s.ArgumentCount)
                            return dt;

                        if (dt.IsNestedType)
                            foreach (var it in dt.ParentType.NestedTypes)
                                if (it.UnoName == dt.UnoName && it.IsGenericDefinition && it.GenericParameters.Length == s.ArgumentCount)
                                    return it;

                        if (dt.IsNamespaceMember)
                            foreach (var it in dt.ParentNamespace.Types)
                                if (it.UnoName == dt.UnoName && it.IsGenericDefinition && it.GenericParameters.Length == s.ArgumentCount)
                                    return it;
                    }

                    if (root is Method)
                    {
                        var method = root as Method;

                        if (method.IsGenericDefinition && method.GenericParameters.Length == s.ArgumentCount)
                            return method;

                        foreach (var m in method.DeclaringType.Methods)
                            if (m.UnoName == method.UnoName && m.IsGenericDefinition && m.GenericParameters.Length == s.ArgumentCount)
                                return m;
                    }

                    return root as InvalidExpression;
                }
                case AstExpressionType.Parameterizer:
                {
                    var s = e as AstParameterizer;
                    var root = ResolveExpression(s.Base, e, scopes);
                    var args = new DataType[s.Arguments.Count];

                    for (int i = 0; i < args.Length; i++)
                    {
                        var a = ResolveExpression(s.Arguments[i], null, scopes);

                        if (a is DataType)
                            args[i] = a as DataType;
                        else
                            return a as InvalidExpression;
                    }

                    if (root is DataType)
                    {
                        var dt = root as DataType;

                        if (dt.IsGenericDefinition && dt.GenericParameters.Length == args.Length)
                            return Parameterize(s.Source, dt, args);

                        if (dt.IsNestedType)
                        {
                            foreach (var it in dt.ParentType.NestedTypes)
                                if (it.UnoName == dt.UnoName && it.IsGenericDefinition && it.GenericParameters.Length == args.Length)
                                    return Parameterize(s.Source, it, args);

                            foreach (var it in dt.ParentType.StrippedTypes)
                                if (it.UnoName == dt.UnoName && it.IsGenericDefinition && it.GenericParameters.Length == args.Length)
                                    return Parameterize(s.Source, it, args);
                        }

                        if (dt.IsNamespaceMember)
                        {
                            foreach (var it in dt.ParentNamespace.Types)
                                if (it.UnoName == dt.UnoName && it.IsGenericDefinition && it.GenericParameters.Length == args.Length)
                                    return Parameterize(s.Source, it, args);

                            foreach (var it in dt.ParentNamespace.StrippedTypes)
                                if (it.UnoName == dt.UnoName && it.IsGenericDefinition && it.GenericParameters.Length == args.Length)
                                    return Parameterize(s.Source, it, args);
                        }
                    }

                    if (root is Method)
                    {
                        var method = root as Method;

                        if (method.IsGenericDefinition && method.GenericParameters.Length == args.Length)
                            return _compiler.TypeBuilder.Parameterize(s.Source, method, args);

                        foreach (var m in method.DeclaringType.Methods)
                            if (m.UnoName == method.UnoName && m.IsGenericDefinition && m.GenericParameters.Length == args.Length)
                                return _compiler.TypeBuilder.Parameterize(s.Source, m, args);
                    }

                    return root as InvalidExpression;
                }
                case AstExpressionType.Call:
                {
                    var s = e as AstCall;
                    var root = ResolveExpression(s.Base, e, scopes);
                    var parameters = new SourceObject[s.Arguments.Count];

                    if (root is Method)
                    {
                        var method = root as Method;

                        if (method.IsGenericDefinition)
                        {
                            var newScopes = new Namescope[scopes.Length + 1];
                            newScopes[0] = method.GenericType;

                            for (int i = 0; i < scopes.Length; i++)
                                newScopes[i + 1] = scopes[i];

                            scopes = newScopes;
                        }
                    }

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var partial = ResolveExpression(s.Arguments[i].Value, e, scopes);

                        if (partial is PartialType)
                            parameters[i] = (partial as PartialType).Type;
                        else
                            parameters[i] = partial ?? DataType.Invalid;
                    }

                    // TODO: Clean up this code

                    if (root is DataType)
                    {
                        foreach (var m in (root as DataType).Constructors)
                            if (TryMatchParameterList(m, s.Arguments, parameters))
                                return m;

                        foreach (var m in (root as DataType).StrippedMembers)
                            if (m is Constructor && TryMatchParameterList(m as Constructor, s.Arguments, parameters))
                                return m;

                        foreach (var m in (root as DataType).Methods)
                            if (m.IsStatic && m.Prototype is Constructor && TryMatchParameterList(m, s.Arguments, parameters))
                                return m;

                        foreach (var m in (root as DataType).StrippedMembers)
                            if (m is Method && m.IsStatic && m.Prototype is Constructor && TryMatchParameterList(m as Method, s.Arguments, parameters))
                                return m;

                        Log.Error(s.Source, ErrorCode.E3363, "The specified constructor could not be found in " + root.Quote());
                        return Expression.Invalid;
                    }

                    if (root is Function)
                    {
                        if (TryMatchParameterList(root as Function, s.Arguments, parameters))
                            return root;

                        if (root is Constructor)
                        {
                            foreach (var m in (root as Constructor).DeclaringType.Constructors)
                                if (TryMatchParameterList(m, s.Arguments, parameters))
                                    return m;

                            foreach (var m in (root as DataType).StrippedMembers)
                                if (m is Constructor && TryMatchParameterList(m as Constructor, s.Arguments, parameters))
                                    return m;

                            Log.Error(s.Source, ErrorCode.E3364, "The specified constructor could not be found in " + (root as Function).DeclaringType.Quote());
                            return Expression.Invalid;
                        }

                        if (root is Method)
                        {
                            var method = root as Method;

                            if (method.IsGenericParameterization)
                            {
                                foreach (var m in method.DeclaringType.Methods)
                                {
                                    if (m.UnoName == method.UnoName &&
                                        m.IsGenericDefinition && m.GenericParameters.Length == method.GenericArguments.Length)
                                    {
                                        var r = _compiler.TypeBuilder.Parameterize(s.Source, m, method.GenericArguments);
                                        if (TryMatchParameterList(r, s.Arguments, parameters))
                                            return r;
                                    }
                                }

                                foreach (var sm in method.DeclaringType.StrippedMembers)
                                {
                                    var m = sm as Method;
                                    if (m != null && m.UnoName == method.UnoName &&
                                        m.IsGenericDefinition && m.GenericParameters.Length == method.GenericArguments.Length)
                                    {
                                        var r = _compiler.TypeBuilder.Parameterize(s.Source, m, method.GenericArguments);
                                        if (TryMatchParameterList(r, s.Arguments, parameters))
                                            return r;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var m in method.DeclaringType.EnumerateFunctions())
                                    if (m.UnoName == method.UnoName &&
                                        !method.IsGenericDefinition && TryMatchParameterList(m, s.Arguments, parameters))
                                        return m;

                                foreach (var m in method.DeclaringType.StrippedMembers)
                                    if (m is Method && m.UnoName == method.UnoName &&
                                        !method.IsGenericDefinition && TryMatchParameterList(m as Method, s.Arguments, parameters))
                                        return m;
                            }
                        }
                        else if (root is Operator)
                        {
                            foreach (var m in (root as Operator).DeclaringType.Operators)
                                if (m.UnoName == (root as Operator).UnoName && TryMatchParameterList(m, s.Arguments, parameters))
                                    return m;

                            foreach (var m in (root as Operator).DeclaringType.StrippedMembers)
                                if (m is Operator && m.UnoName == (root as Operator).UnoName && TryMatchParameterList(m as Operator, s.Arguments, parameters))
                                    return m;
                        }
                        else if (root is Cast)
                        {
                            foreach (var m in (root as Cast).DeclaringType.Casts)
                                if (m.UnoName == (root as Cast).UnoName && TryMatchParameterList(m, s.Arguments, parameters))
                                    return m;

                            foreach (var m in (root as Cast).DeclaringType.StrippedMembers)
                                if (m is Cast && m.UnoName == (root as Cast).UnoName && TryMatchParameterList(m as Cast, s.Arguments, parameters))
                                    return m;
                        }

                        // TODO: Should be recursive in cases where a qualified base expression is used as an instance

                        Log.Error(s.Source, ErrorCode.E3365, "The specified overload of " + (root as Function).UnoName.Quote() + " could not be found in " + (root as Function).DeclaringType.Quote());
                        return Expression.Invalid;
                    }

                    break;
                }
                case AstExpressionType.Member:
                {
                    var s = (AstMember) e;
                    var root = ResolveExpression(s.Base, e, scopes);
                    return TryResolveEntity(s.Source, root as Namescope, s.Name.Symbol, p);
                }
                case AstExpressionType.Identifier:
                {
                    var s = (AstIdentifier) e;

                    foreach (var root in scopes)
                    {
                        var result = TryResolveEntity(s.Source, root, s.Symbol, p);
                        if (result != null)
                            return result;
                    }

                    return TryResolveEntity(s.Source, _il, s.Symbol, p);
                }

                case AstExpressionType.Global:
                    return _il;
            }

            return null;
        }
    }
}
