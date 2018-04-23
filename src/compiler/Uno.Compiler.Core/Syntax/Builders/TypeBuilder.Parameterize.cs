using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder
    {
        public DataType Parameterize(DataType dt)
        {
            if (!dt.IsFlattenedDefinition)
                return dt;

            var pd = dt.Parent as DataType;

            if (pd != null && pd.IsFlattenedDefinition)
            {
                var pp = Parameterize(pd);

                foreach (var it in pp.NestedTypes)
                    if (it.MasterDefinition == dt.MasterDefinition)
                        return Parameterize(it);

                throw new FatalException(dt.Source, ErrorCode.I3044, "Unable to find parameterized version of " + dt.Quote());
            }

            var pt = Parameterize(dt.Source, dt, dt.GenericParameters);
            pt.Stats |= EntityStats.ParameterizedDefinition;
            return pt;
        }

        public DataType Parameterize(Source src, DataType definition, params DataType[] args)
        {
            if (args.Length != definition.GenericParameters.Length)
            {
                Log.Error(src, ErrorCode.I0000, "pt.GenericTypeArguments.Length != dt.GenericTypeParameters.Length");
                return DataType.Invalid;
            }

            foreach (var p in definition.GenericParameterizations)
            {
                var found = true;
                var pargs = p.GenericArguments;

                if (pargs.Length != args.Length)
                    throw new FatalException(definition.Source, ErrorCode.I3036, "Invalid argument count");

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] != pargs[i])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                    return p;
            }

            for (int i = 0; i < args.Length; i++)
            {
                var gt = definition.GenericParameters[i];
                var at = args[i];

                switch (at.TypeType)
                {
                    case TypeType.Invalid:
                        continue;

                    case TypeType.GenericParameter:
                        // Disables generic argument type validation for overridden and interface implementing generic methods,
                        //       as they in some cases inherit their base generic constaints which are handled and validated somewhere else.
                        // TODO: This is a also a workaround that might cause other problems. Verify and fix properly later.
                        continue;

                    case TypeType.Enum:
                    case TypeType.Struct:
                    case TypeType.Class:
                    case TypeType.RefArray:
                    case TypeType.Delegate:
                    case TypeType.Interface:
                        {
                            at.AssignAttributes();

                            // TODO: Commenting this out due to problems with circular reference, this is also verified by ILVerifier, though the error messages will suck.

                            /*
                            //AssignBaseType(at);
                            //PopulateTypeMembers(at);

                            switch (gt.ConstraintType)
                            {
                                case GenericConstraintType.Class:
                                    if (!at.IsReferenceType())
                                    {
                                        Log.ReportError(src, ErrorCode.E0000, at.Quoted() + " cannot be used as generic type argument for " + gt.Quoted() + " because it is not a reference type");
                                        args[i] = DataType.Invalid;
                                        continue;
                                    }
                                    else if (!at.IsEqualOrSubclassOf(gt.BaseType))
                                    {
                                        Log.ReportError(src, ErrorCode.E0000, at.Quoted() + " cannot be used as generic type argument for " + gt.Quoted() + " because it does not inherit " + gt.BaseType.Quoted());
                                        args[i] = DataType.Invalid;
                                        continue;
                                    }

                                    break;

                                case GenericConstraintType.Struct:
                                    if (!at.IsValueType())
                                    {
                                        Log.ReportError(src, ErrorCode.E0000, at.Quoted() + " cannot be used as generic type argument for " + gt.Quoted() + " because it is not a value type");
                                        args[i] = DataType.Invalid;
                                        continue;
                                    }

                                    break;
                            }

                            var defCtor = at.TryFindDefaultConstructor();

                            if (gt.Constructors.Count > 0 && (defCtor == null || !defCtor.IsPublic))
                            {
                                Log.ReportError(src, ErrorCode.E0000, at.Quoted() + " cannot be used as generic type argument for " + gt.Quoted() + " because it does not provide a public parameterless constructor");
                                args[i] = DataType.Invalid;
                                continue;
                            }

                            foreach (var it in gt.InterfaceTypes)
                            {
                                if (!at.IsImplementingInterface(it))
                                {
                                    Log.ReportError(src, ErrorCode.E0000, at.Quoted() + " cannot be used as generic type argument for " + gt.Quoted() + " because it does not implement the interface " + it.Quoted());
                                    args[i] = DataType.Invalid;
                                    continue;
                                }
                            }
                            */
                        }
                        break;

                    default:
                        Log.Error(src, ErrorCode.E0000, at.Quote() + " cannot be used as generic type argument");
                        args[i] = DataType.Invalid;
                        continue;
                }
            }

            if (Log.HasErrors)
            {
                foreach (var p in definition.GenericParameterizations)
                {
                    var found = true;
                    var pargs = p.GenericArguments;

                    if (pargs.Length != args.Length)
                        throw new FatalException(definition.Source, ErrorCode.I3036, "Invalid argument count");

                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] != pargs[i])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return p;
                }
            }

            var result = definition.CreateParameterization(args);
            var map = new Dictionary<DataType, DataType>();

            for (int i = 0; i < definition.GenericParameters.Length; i++)
                map.Add(definition.GenericParameters[i], result.GenericArguments[i]);

            result.SetMasterDefinition(definition.MasterDefinition);
            ParameterizeInnerTypes(definition, definition, map, result);
            EnqueueType(result,
                x => ParameterizeBaseType(definition, map, x),
                x => ParameterizeMembers(definition, definition, map, x));
            return result;
        }

        public Method Parameterize(Source src, Method definition, params DataType[] args)
        {
            var dt = Parameterize(src, definition.GenericType, args);
            dt.PopulateMembers();
            return dt.Methods[0];
        }

        DataType ParameterizeType(Dictionary<DataType, DataType> map, DataType arg)
        {
            if (arg == null)
                return null;

            DataType result;
            if (map.TryGetValue(arg, out result))
                return result;

            if (arg is RefArrayType)
            {
                result = GetArray(ParameterizeType(map, arg.ElementType));
            }
            else if (arg is FixedArrayType)
            {
                result = new FixedArrayType(
                    arg.Source,
                    ParameterizeType(map, arg.ElementType),
                    ((FixedArrayType)arg).OptionalSize,
                    _ilf.Essentials.Int);
            }
            else if (arg.IsNestedType && arg.ParentType.IsFlattenedParameterization)
            {
                var p = ParameterizeType(map, arg.ParentType);

                if (arg.IsGenericParameter && p.IsGenericDefinition)
                {
                    // Should not be necessary
                    foreach (var pp in p.GenericParameters)
                    {
                        if (pp.UnoName == arg.UnoName)
                        {
                            result = pp;
                            break;
                        }
                    }
                }
                else
                {
                    bool innerTypeFound = false;

                    foreach (var it in p.NestedTypes)
                    {
                        if (arg.MasterDefinition == it.MasterDefinition)
                        {
                            if (it.IsGenericDefinition)
                            {
                                var pargs = new DataType[arg.GenericArguments.Length];

                                for (int i = 0, l = pargs.Length; i < l; i++)
                                    pargs[i] = ParameterizeType(map, arg.GenericArguments[i]);

                                result = Parameterize(it.Source, it, pargs);
                            }
                            else
                            {
                                result = it;
                            }

                            innerTypeFound = true;
                            break;
                        }
                    }

                    if (!innerTypeFound)
                    {
                        // Should not be necessary
                        foreach (var m in p.Methods)
                        {
                            if (m.IsGenericDefinition &&
                                m.GenericType.MasterDefinition == arg.MasterDefinition)
                            {
                                result = m.GenericType;
                                break;
                            }
                        }
                    }
                }
            }
            else if (arg.IsGenericParameterization)
            {
                var pargs = new DataType[arg.GenericArguments.Length];

                for (int i = 0, l = pargs.Length; i < l; i++)
                    pargs[i] = ParameterizeType(map, arg.GenericArguments[i]);

                result = Parameterize(arg.Source, arg.GenericDefinition, pargs);
            }
            else
                result = arg;

            if (result != null)
            {
                map.Add(arg, result);
                return result;
            }

            throw new FatalException(arg.Source, ErrorCode.I0000, "Failed to parameterize " + arg.Quote());
        }

        Parameter[] ParameterizeParameters(Dictionary<DataType, DataType> map, params Parameter[] parameters)
        {
            var result = new Parameter[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                result[i] = new Parameter(
                    parameters[i].Source,
                    parameters[i].Attributes,
                    parameters[i].Modifier,
                    ParameterizeType(map, parameters[i].Type),
                    parameters[i].Name,
                    ParameterizeDefault(map, parameters[i].OptionalDefault));

            return result;
        }

        Expression ParameterizeDefault(Dictionary<DataType, DataType> map, Expression e)
        {
            if (e == null)
                return null;

            var t = ParameterizeType(map, e.ReturnType);

            switch (e.ExpressionType)
            {
                case ExpressionType.Default:
                    return t.IsReferenceType
                        ? (Expression) new Constant(e.Source, t, null)
                        :              new Default(e.Source, t);
                case ExpressionType.Constant:
                    return new Constant(e.Source, t, e.ConstantValue);
                default:
                    Log.Warning(e.Source, ErrorCode.I0000, "Failed to parameterize parameter value: " + e);
                    return e;
            }
        }

        DataType CreateParameterizableInnerType(DataType oldInnerType, DataType newParent)
        {
            switch (oldInnerType.TypeType)
            {
                case TypeType.Class:
                    return new ClassType(oldInnerType.Source, newParent, oldInnerType.DocComment, oldInnerType.Modifiers, oldInnerType.UnoName);
                case TypeType.Interface:
                    return new InterfaceType(oldInnerType.Source, newParent, oldInnerType.DocComment, oldInnerType.Modifiers, oldInnerType.UnoName);
                case TypeType.Delegate:
                    return new DelegateType(oldInnerType.Source, newParent, oldInnerType.DocComment, oldInnerType.Modifiers, oldInnerType.UnoName);
                case TypeType.Struct:
                    return new StructType(oldInnerType.Source, newParent, oldInnerType.DocComment, oldInnerType.Modifiers, oldInnerType.UnoName);
                case TypeType.Enum:
                    return new EnumType(oldInnerType.Source, newParent, oldInnerType.DocComment, oldInnerType.Modifiers, oldInnerType.UnoName);
                default:
                    throw new FatalException(oldInnerType.Source, ErrorCode.I0000, "Failed to create parameterizable generic inner type");
            }
        }

        void ParameterizeInnerTypes(DataType definition, DataType current, Dictionary<DataType, DataType> map, DataType result)
        {
            for (int i = 0; i < current.NestedTypes.Count; i++)
            {
                var e = current.NestedTypes[i];
                var t = CreateParameterizableInnerType(e, result);

                if (e.IsGenericDefinition)
                    t.MakeGenericDefinition(e.GenericParameters);

                t.SetMasterDefinition(e.MasterDefinition);

                EnqueueType(t,
                    x => ParameterizeBaseType(e, map, x),
                    x => ParameterizeMembers(definition, e, map, x));
                ParameterizeInnerTypes(definition, e, map, t);
                result.NestedTypes.Add(t);
            }
        }

        Method ParameterizeMethod(Dictionary<DataType, DataType> map, Method m)
        {
            if (m == null)
                return null;

            var dt = ParameterizeType(map, m.DeclaringType);
            dt.PopulateMembers();

            foreach (var f in dt.Methods)
            {
                if (m.MasterDefinition == f.MasterDefinition)
                {
                    if (m.IsGenericParameterization)
                    {
                        var pargs = new DataType[m.GenericArguments.Length];

                        for (int i = 0, l = pargs.Length; i < l; i++)
                            pargs[i] = ParameterizeType(map, m.GenericArguments[i]);

                        return Parameterize(m.Source, f, pargs);
                    }

                    return f;
                }
            }

            throw new FatalException(m.Source, ErrorCode.I3031, "Unable to find parameterized method: " + m);
        }

        Property ParameterizeProperty(Dictionary<DataType, DataType> map, Property m)
        {
            if (m == null)
                return null;

            var dt = ParameterizeType(map, m.DeclaringType);
            dt.PopulateMembers();

            foreach (var f in dt.Properties)
                if (m.MasterDefinition == f.MasterDefinition)
                    return f;

            throw new FatalException(m.Source, ErrorCode.I3033, "Unable to find parameterized property: " + m);
        }

        Event ParameterizeEvent(Dictionary<DataType, DataType> map, Event m)
        {
            if (m == null)
                return null;

            var dt = ParameterizeType(map, m.DeclaringType);
            dt.PopulateMembers();

            foreach (var f in dt.Events)
                if (m.MasterDefinition == f.MasterDefinition)
                    return f;

            throw new FatalException(m.Source, ErrorCode.I3034, "Unable to find parameterized event: " + m);
        }

        void ParameterizeBaseType(DataType current, Dictionary<DataType, DataType> map, DataType result)
        {
            current.AssignBaseType();
            result.SetBase(ParameterizeType(map, current.Base));

            if (result is DelegateType)
            {
                var currentDelegate = (DelegateType)current;
                var resultDelegate = (DelegateType)result;
                resultDelegate.SetReturnType(ParameterizeType(map, currentDelegate.ReturnType));
                resultDelegate.SetParameters(ParameterizeParameters(map, currentDelegate.Parameters));
            }
            else if (current.Interfaces.Length > 0)
            {
                var interfaceTypes = new InterfaceType[current.Interfaces.Length];

                for (int i = 0, l = interfaceTypes.Length; i < l; i++)
                    interfaceTypes[i] = (InterfaceType)ParameterizeType(map, current.Interfaces[i]);

                result.SetInterfaces(interfaceTypes);
            }
        }

        void ParameterizeMembers(DataType definition, DataType current, Dictionary<DataType, DataType> map, DataType result)
        {
            current.AssignAttributes();
            result.SetAttributes(current.Attributes);
            current.PopulateMembers();

            foreach (var s in current.Swizzlers)
                result.Swizzlers.Add(ParameterizeType(map, s));

            if (current.Initializer != null)
            {
                var m = current.Initializer;
                var c = new Constructor(m.Source, result, m.DocComment,
                    m.Modifiers, ParameterizeParameters(map, m.Parameters), m.Body);
                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Initializer = c;
            }

            if (current.Finalizer != null)
            {
                var m = current.Finalizer;
                var c = new Finalizer(m.Source, result, m.DocComment,
                    m.Modifiers, ParameterizeParameters(map, m.Parameters), m.Body);
                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Finalizer = c;
            }

            foreach (var m in current.Constructors)
            {
                var c = new Constructor(m.Source, result, m.DocComment,
                    m.Modifiers, ParameterizeParameters(map, m.Parameters), m.Body);
                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Constructors.Add(c);
            }

            foreach (var m in current.Methods)
            {
                DataType owner = result;
                ClassType genericMethodParametersOwner = null;

                if (m.IsGenericDefinition)
                {
                    if (m.GenericType != definition)
                    {
                        genericMethodParametersOwner = new ClassType(m.Source, result, m.DocComment, Modifiers.Private | Modifiers.Static | Modifiers.Generated, m.UnoName);
                        genericMethodParametersOwner.MakeGenericDefinition(m.GenericParameters);

                        foreach (var p in m.GenericParameters)
                            map.Add(p, p);
                    }
                    else
                    {
                        owner = (DataType)result.Parent;
                        genericMethodParametersOwner = (ClassType)result;
                    }
                }

                var c = new Method(m.Source, owner, m.DocComment,
                    m.Modifiers, m.UnoName, genericMethodParametersOwner, ParameterizeType(map, m.ReturnType), ParameterizeParameters(map, m.Parameters), m.Body);

                if (genericMethodParametersOwner != null && genericMethodParametersOwner != result)
                    genericMethodParametersOwner.Methods.Add(c);

                if (m.OverriddenMethod != null)
                    c.SetOverriddenMethod(ParameterizeMethod(map, m.OverriddenMethod));
                if (m.ImplementedMethod != null)
                    c.SetImplementedMethod(ParameterizeMethod(map, m.ImplementedMethod));

                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Methods.Add(c);
            }

            foreach (var m in current.Properties)
            {
                var c = new Property(m.Source, m.DocComment, m.Modifiers, m.UnoName, result,
                    ParameterizeType(map, m.ReturnType), ParameterizeParameters(map, m.Parameters));

                if (m.GetMethod != null)
                    c.CreateGetMethod(m.GetMethod.Source, m.GetMethod.Modifiers, m.GetMethod.Body);
                if (m.SetMethod != null)
                    c.CreateSetMethod(m.SetMethod.Source, m.SetMethod.Modifiers, m.SetMethod.Body);
                if (m.ImplicitField != null)
                    c.CreateImplicitField(m.ImplicitField.Source);

                if (m.OverriddenProperty != null)
                    c.SetOverriddenProperty(ParameterizeProperty(map, m.OverriddenProperty));
                if (m.ImplementedProperty != null)
                    c.SetImplementedProperty(ParameterizeProperty(map, m.ImplementedProperty));

                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Properties.Add(c);
            }

            foreach (var m in current.Events)
            {
                var c = new Event(m.Source, m.DocComment, m.Modifiers, result,
                    ParameterizeType(map, m.ReturnType), m.UnoName);

                if (m.AddMethod != null)
                    c.CreateAddMethod(m.AddMethod.Source, m.AddMethod.Modifiers, m.AddMethod.Body);
                if (m.RemoveMethod != null)
                    c.CreateRemoveMethod(m.RemoveMethod.Source, m.RemoveMethod.Modifiers, m.RemoveMethod.Body);
                if (m.ImplicitField != null)
                    c.CreateImplicitField(m.ImplicitField.Source);

                if (m.OverriddenEvent != null)
                    c.SetOverriddenEvent(ParameterizeEvent(map, m.OverriddenEvent));
                if (m.ImplementedEvent != null)
                    c.SetImplementedEvent(ParameterizeEvent(map, m.ImplementedEvent));

                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Events.Add(c);
            }

            foreach (var m in current.Casts)
            {
                var c = new Cast(m.Source, result, m.Type, m.DocComment, m.Modifiers,
                    ParameterizeType(map, m.ReturnType), ParameterizeParameters(map, m.Parameters), m.Body);
                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Casts.Add(c);
            }

            foreach (var m in current.Operators)
            {
                var c = new Operator(m.Source, result, m.Type, m.DocComment, m.Modifiers,
                    ParameterizeType(map, m.ReturnType), ParameterizeParameters(map, m.Parameters), m.Body);
                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Operators.Add(c);
            }

            foreach (var m in current.Fields)
            {
                var c = new Field(m.Source, result, m.UnoName, m.DocComment, m.Modifiers, m.FieldModifiers,
                    ParameterizeType(map, m.ReturnType));
                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Fields.Add(c);
            }

            foreach (var m in current.Literals)
            {
                var c = new Literal(m.Source, result, m.UnoName,m.DocComment,  m.Modifiers,
                    ParameterizeType(map, m.ReturnType), m.Value);
                m.AssignAttributes();
                c.SetAttributes(m.Attributes);
                c.SetMasterDefinition(m.MasterDefinition);
                result.Literals.Add(c);
            }

            result.AssignBaseType();

            if (!result.IsParameterizedDefinition &&
                !result.IsInterface && result.Interfaces.Length > 0)
            {
                foreach (var e in current.InterfaceMethods)
                {
                    var impl = FindParameterizedMethod(ParameterizeType(map, e.Key.DeclaringType), e.Key);
                    var decl = FindParameterizedMethod(result, e.Value);
                    result.InterfaceMethods[impl] = decl;
                }
            }
        }

        Method FindParameterizedMethod(DataType pt, Method def)
        {
            // TODO: Why is this true?
            if (def.DeclaringType.MasterDefinition != pt.MasterDefinition)
                return def;

            pt.PopulateMembers();
            foreach (var f in pt.EnumerateFunctions())
                if (f.MasterDefinition == def.MasterDefinition)
                    return (Method)f;

            throw new FatalException(def.Source, ErrorCode.I0000, "Unable to find parameterized " + def.Quote() + " in " + pt.Quote());
        }
    }
}
