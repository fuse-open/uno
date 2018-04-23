using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        public DataType TryGetImplicitElementType(Expression[] values)
        {
            DataType et = null;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ReturnType.IsReferenceType ||
                    values[i].ReturnType.IsValueType ||
                    values[i].ReturnType.IsGenericParameter)
                {
                    et = values[i].ReturnType;
                    break;
                }
            }

            if (et != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (!et.Equals(values[i].ReturnType))
                    {
                        var ic = TryCompileImplicitCast(values[i].Source, et, values[i]);
                        if (ic != null)
                            continue;

                        bool valid = true;
                        et = values[i].ReturnType;

                        for (int j = 0; j < i; j++)
                        {
                            var jc = TryCompileImplicitCast(values[j].Source, et, values[j]);
                            if (jc != null)
                                continue;
                            valid = false;
                            break;
                        }

                        if (valid)
                            continue;
                        return null;
                    }
                }
            }

            return et;
        }

        public Expression CompileAddToCollection(string collecionName, AstExpression initializer)
        {
            AstArgument[] addArgs;

            if (initializer is AstArrayInitializer)
            {
                var ai = initializer as AstArrayInitializer;
                if (ai.Values.Count == 0)
                    return Error(initializer.Source, ErrorCode.E2079, "Element initializer cannot be empty");

                addArgs = new AstArgument[ai.Values.Count];

                for (int j = 0; j < addArgs.Length; j++)
                    addArgs[j] = ai.Values[j];
            }
            else
                addArgs = new AstArgument[] {initializer};

            return CompileCall(
                new AstCall(
                    AstCallType.Function,
                    new AstMember(
                        new AstIdentifier(initializer.Source, collecionName),
                        new AstIdentifier(initializer.Source, "Add")),
                    addArgs));
        }

        public Expression CompileNewExpression(AstNew e)
        {
            // Implicitly typed arrays
            if (e.OptionalType == null)
            {
                if (e.OptionalArguments != null)
                    return Error(e.Source, ErrorCode.E2001, "Array constructors cannot have argument list");
                if (e.OptionalArraySize != null)
                    return Error(e.Source, ErrorCode.E2002, "Cannot specify size on implicitly typed arrays");
                if (e.OptionalCollectionInitializer == null || e.OptionalCollectionInitializer.Count == 0)
                    return Error(e.Source, ErrorCode.E2003, "Must provide non-empty initializer list for implicitly typed arrays");

                var values = new Expression[e.OptionalCollectionInitializer.Count];
                for (int i = 0; i < values.Length; i++)
                    values[i] = CompileExpression(e.OptionalCollectionInitializer[i]);

                var et = TryGetImplicitElementType(values);

                if (et == null)
                    return Error(e.Source, ErrorCode.E2004, "No best type found for implicitly typed array");

                for (int i = 0; i < values.Length; i++)
                    values[i] = CompileImplicitCast(e.Source, et, values[i]);

                return et.IsValueType || et.IsReferenceType
                        ? new NewArray(e.Source, TypeBuilder.GetArray(et), values) :
                    et != DataType.Invalid
                        ? Error(e.Source, ErrorCode.E2080, "Cannot create an implicitly typed array of type " + et.Quote())
                        : Expression.Invalid;
            }

            var dt = NameResolver.GetType(Namescope, e.OptionalType);

            switch (dt.TypeType)
            {
                case TypeType.RefArray:
                {
                    if (e.OptionalArguments != null)
                        return Error(e.Source, ErrorCode.E2005, "Array constructors cannot have argument list");

                    var at = dt as RefArrayType;
                    Expression size = null;

                    if (e.OptionalArraySize != null)
                    {
                        size = CompileImplicitCast(e.Source, Essentials.Int, CompileExpression(e.OptionalArraySize));
                        if (size.IsInvalid)
                            return Expression.Invalid;
                    }

                    if (e.OptionalCollectionInitializer != null)
                    {
                        var values = new Expression[e.OptionalCollectionInitializer.Count];
                        for (int i = 0; i < values.Length; i++)
                            values[i] = CompileExpression(e.OptionalCollectionInitializer[i]);

                        if (size != null)
                        {
                            var c = Compiler.ConstantFolder.TryMakeConstant(size);
                            if (c == null || !c.Value.Equals(values.Length))
                                return Error(size.Source, ErrorCode.E2006, "Inconsistent array size and initializer list length");
                        }

                        switch (at.ElementType.BuiltinType)
                        {
                            case BuiltinType.Bool:
                            case BuiltinType.Byte:
                            case BuiltinType.Char:
                            case BuiltinType.Double:
                            case BuiltinType.Int:
                            case BuiltinType.Float:
                            case BuiltinType.Long:
                            case BuiltinType.SByte:
                            case BuiltinType.Short:
                            case BuiltinType.UInt:
                            case BuiltinType.ULong:
                            case BuiltinType.UShort:
                                // Disable warning on primitive types
                                break;

                            default:
                                if (TryGetImplicitElementType(values) == at.ElementType)
                                    Log.Warning3(e.Source, ErrorCode.W2007, "Array can be instantiated as implicitly typed array (new[] { ... })");

                                break;
                        }

                        for (int i = 0; i < values.Length; i++)
                            values[i] = CompileImplicitCast(values[i].Source, at.ElementType, values[i]);

                        return new NewArray(e.Source, at, values);
                    }

                    return new NewArray(e.Source, at, size);
                }
                case TypeType.Class:
                case TypeType.Struct:
                case TypeType.GenericParameter:
                {
                    if (dt.IsStatic)
                        return Error(e.Source, ErrorCode.E2090, "Cannot instantiate static class");

                    if (e.OptionalArguments != null || e.OptionalCollectionInitializer != null)
                    {
                        if (e.OptionalArraySize != null)
                            return Error(e.Source, ErrorCode.E2008, "Object constructors cannot have array size");

                        Expression newObject;

                        if (dt.IsStruct && (e.OptionalArguments == null || e.OptionalArguments.Count == 0))
                            newObject = new Default(e.Source, dt);
                        else
                        {
                            dt.PopulateMembers();

                            Constructor ctor;
                            Expression[] args;

                            if (!TryResolveConstructorOverload(e.Source, dt.Constructors, e.OptionalArguments ?? AstArgument.Empty, out ctor, out args))
                                return e.OptionalArguments != null && e.OptionalArguments.Count > 0
                                    ? (dt.Constructors.Count == 1
                                        ? Error(e.Source, ErrorCode.E2009, "Call to " + (dt + PrintableParameterList(dt.Constructors[0].Parameters)).Quote() + " has some invalid arguments " +
                                            PrintableArgumentList(e.OptionalArguments))
                                        : Error(e.Source, ErrorCode.E2009, dt.Quote() + " has no constructors matching the argument list " +
                                            PrintableArgumentList(e.OptionalArguments) +
                                            NameResolver.SuggestCandidates(dt.Constructors)))
                                    : Error(e.Source, ErrorCode.E0000, dt.Quote() + " has no default constructor");

                            newObject = new NewObject(e.Source, ctor, args);
                        }

                        if (e.OptionalCollectionInitializer != null && e.OptionalCollectionInitializer.Count > 0)
                        {
                            var var = new Variable(e.Source, Function, Namescope.GetUniqueIdentifier("collection"), newObject.ReturnType);
                            CurrentVariableScope.Variables.Add(var.Name, var);

                            Expression root = new StoreLocal(e.Source, var, newObject);

                            // See if it is a member initializer or collection initializer
                            var containsAssignOp = false;

                            foreach (var i in e.OptionalCollectionInitializer)
                            {
                                if (i is AstBinary && (i as AstBinary).IsAssign)
                                {
                                    containsAssignOp = true;
                                    break;
                                }
                            }

                            if (containsAssignOp)
                            {
                                var initedMembers = new List<string>();

                                // Assign members
                                foreach (var i in e.OptionalCollectionInitializer)
                                {
                                    var binOp = i as AstBinary;
                                    if (binOp == null || binOp.Type != AstBinaryType.Assign || !(binOp.Left is AstIdentifier))
                                    {
                                        Log.Error(i.Source, ErrorCode.E2077, "Invalid initalizer member declarator");
                                        continue;
                                    }

                                    var id = (AstIdentifier)binOp.Left;
                                    var member = new AstMember(new AstIdentifier(i.Source, var.Name), id);

                                    if (binOp.Right is AstArrayInitializer)
                                    {
                                        var ai = binOp.Right as AstArrayInitializer;
                                        var collection = CompileExpression(member);

                                        var cvar = new Variable(e.Source, Function, Namescope.GetUniqueIdentifier("array"), collection.ReturnType);
                                        CurrentVariableScope.Variables.Add(cvar.Name, cvar);

                                        root = new SequenceOp(root, new StoreLocal(collection.Source, cvar, collection));

                                        if (collection.IsInvalid)
                                            continue;

                                        foreach (var ci in ai.Values)
                                            root = new SequenceOp(root, CompileAddToCollection(cvar.Name, ci));
                                    }
                                    else
                                    {
                                        var assign = CompileAssign(new AstBinary(AstBinaryType.Assign, member, i.Source, binOp.Right));
                                        root = new SequenceOp(root, assign);

                                        if (assign.IsInvalid)
                                            continue;
                                    }

                                    foreach (var m in initedMembers)
                                    {
                                        if (m == id.Symbol)
                                        {
                                            Log.Error(i.Source, ErrorCode.E2078, "Duplicate initialization of member " + id.Symbol.Quote());
                                            break;
                                        }
                                    }

                                    initedMembers.Add(id.Symbol);
                                }
                            }
                            else
                            {
                                // Add to collection
                                foreach (var i in e.OptionalCollectionInitializer)
                                    root = new SequenceOp(root, CompileAddToCollection(var.Name, i));
                            }

                            return new SequenceOp(root, new LoadLocal(e.Source, var));
                        }

                        if (e.OptionalType is AstBuiltinType && dt.IsStruct)
                            Log.Warning(e.Source, ErrorCode.W0000, "Redundant 'new' operator on builtin struct initialization");

                        return newObject;
                    }

                    return Error(e.Source, ErrorCode.E2011, "Must provide argument list for object constructor");
                }
                case TypeType.Delegate:
                {
                    if (e.OptionalArguments == null)
                        return Error(e.Source, ErrorCode.E0000, "Must provide argument list for delegate constructor");

                    var args = CompileArgumentList(e.OptionalArguments);

                    if (e.OptionalCollectionInitializer != null)
                        return Error(e.Source, ErrorCode.E0000, "Delegate construction cannot have collection initializer");
                    if (e.OptionalArraySize != null)
                        return Error(e.Source, ErrorCode.E0000, "Delegate construction cannot have array size");
                    if (args.Length != 1 || args[0].ExpressionType != ExpressionType.MethodGroup)
                        return Error(e.Source, ErrorCode.E0000, "Delegate construction requires one method argument");

                    return CompileImplicitCast(e.Source, dt, args[0]);
                }

                case TypeType.Invalid:
                    return Expression.Invalid;

                default:
                    return Error(e.Source, ErrorCode.E2012, "Instances of type " + dt.Quote() + " cannot be created using 'new' because it is not a class, struct or array type");
            }
        }
    }
}
