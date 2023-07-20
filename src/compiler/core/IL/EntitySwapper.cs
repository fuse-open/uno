using System.Collections.Generic;
using Uno;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL
{
    abstract class EntitySwapper : CompilerPass
    {
        public readonly Dictionary<DataType, DataType> SwapTypes = new Dictionary<DataType, DataType>();
        public readonly Dictionary<Member, Member> SwapMembers = new Dictionary<Member, Member>();

        protected EntitySwapper(CompilerPass parent)
            : base(parent)
        {
        }

        DataType GetTypeInternal(DataType dt)
        {
            DataType result;
            if (SwapTypes.TryGetValue(dt, out result))
                return result;

            switch (dt.TypeType)
            {
                case TypeType.GenericParameter:
                {
                    SwapTypes.Add(dt, dt);
                    return dt;
                }
                case TypeType.RefArray:
                {
                    var at = TypeBuilder.GetArray(GetType(dt.ElementType));
                    SwapTypes.Add(dt, at);
                    return at;
                }
                case TypeType.FixedArray:
                {
                    var at = (FixedArrayType)dt;
                    at = new FixedArrayType(at.Source, GetType(at.ElementType), at.OptionalSize, Essentials.Int);
                    SwapTypes.Add(dt, at);
                    return at;
                }
            }

            result = dt;

            if (dt.IsNestedType)
            {
                if (!dt.IsGenericParameterization && dt.IsFlattenedParameterization)
                {
                    var def = GetType(dt.MasterDefinition);

                    // Transformed type was unnested
                    if (!def.IsNestedType)
                    {
                        if (def.IsGenericDefinition)
                        {
                            var args = new DataType[dt.FlattenedArguments.Length];

                            for (int i = 0; i < dt.FlattenedArguments.Length; i++)
                                args[i] = GetType(dt.FlattenedArguments[i]);

                            result = TypeBuilder.Parameterize(result.Source, def, args);
                            TypeBuilder.BuildTypes();
                            SwapTypes.Add(dt, result);
                            return result;
                        }

                        // Unnested enum
                        SwapTypes.Add(dt, def);
                        return def;
                    }
                }

                var pt = GetType(dt.ParentType);
                if (pt != dt.Parent)
                {
                    foreach (var it in pt.NestedTypes)
                    {
                        if (it.MasterDefinition == dt.MasterDefinition)
                        {
                            result = pt;
                            break;
                        }
                    }
                }
            }

            if (dt.IsGenericParameterization)
            {
                var args = new DataType[dt.GenericArguments.Length];

                for (int i = 0; i < dt.GenericArguments.Length; i++)
                    args[i] = GetType(dt.GenericArguments[i]);

                var def = result.IsGenericDefinition ? result : GetType(dt.GenericDefinition);
                result = TypeBuilder.Parameterize(dt.Source, def, args);
                TypeBuilder.BuildTypes();
            }

            SwapTypes.Add(dt, result);
            return result;
        }

        public DataType GetType(DataType dt)
        {
            var r = GetTypeInternal(dt);
            return r != dt && !r.IsArray ? GetType(r) : r;
        }

        DataType GetObjectType(Expression obj, DataType type)
        {
            return
                obj != null && !type.IsInterface ?
                    obj.ReturnType :
                    GetType(type);
        }

        public Parameter[] GetParameterList(Parameter[] pl)
        {
            var result = new Parameter[pl.Length];

            for (int i = 0; i < pl.Length; i++)
            {
                var p = pl[i];
                result[i] = new Parameter(p.Source, p.Attributes, p.Modifier, GetType(p.Type), p.Name, null);
            }

            return result;
        }

        public Constructor GetConstructor(Constructor m)
        {
            if (m == null)
                return null;

            Member result;
            if (SwapMembers.TryGetValue(m, out result))
                return (Constructor)result;

            var t0 = m.DeclaringType;
            var t1 = GetType(t0);

            if (t0 != t1)
            {
                if (t0.IsGenericParameter)
                {
                    foreach (var e in t1.Constructors)
                        if (e.CompareParameters(m))
                            return e;
                }
                else
                {
                    foreach (var e in t1.Constructors)
                        if (e.MasterDefinition == m.MasterDefinition)
                            return e;
                }

                Log.Warning(m.Source, ErrorCode.I0000, m.Quote() + " was not found");
            }

            return m;
        }

        public Operator GetOperator(Operator m)
        {
            if (m == null)
                return null;

            Member result;
            if (SwapMembers.TryGetValue(m, out result))
                return (Operator)result;

            var t0 = m.DeclaringType;
            var t1 = GetType(t0);

            if (t0 != t1)
            {
                foreach (var e in t1.Operators)
                    if (e.MasterDefinition == m.MasterDefinition)
                        return e;

                Log.Warning(m.Source, ErrorCode.I0000, m.Quote() + " was not found");
            }

            return m;
        }

        public Cast GetCast(Cast m)
        {
            if (m == null)
                return null;

            Member result;
            if (SwapMembers.TryGetValue(m, out result))
                return (Cast)result;

            var t0 = m.DeclaringType;
            var t1 = GetType(t0);

            if (t0 != t1)
            {
                foreach (var e in t1.Casts)
                    if (e.MasterDefinition == m.MasterDefinition)
                        return e;

                Log.Warning(m.Source, ErrorCode.I0000, m.Quote() + " was not found");
            }

            return m;
        }

        Method GetMethodInternal(Method m, Expression obj)
        {
            if (m == null)
                return null;

            Member result;
            if (SwapMembers.TryGetValue(m, out result))
                return (Method)result;

            var t0 = m.DeclaringType;
            var t1 = GetObjectType(obj, t0);

            if (t0 != t1)
            {
                for (var t = t1; t != null; t = t.Base)
                {
                    foreach (var e in t.Methods)
                    {
                        if (e.MasterDefinition == m.MasterDefinition)
                        {
                            if (m.IsGenericParameterization)
                            {
                                var args = new DataType[m.GenericArguments.Length];

                                for (int i = 0; i < args.Length; i++)
                                    args[i] = GetType(m.GenericArguments[i]);

                                return
                                    e.IsGenericDefinition
                                        ? TypeBuilder.Parameterize(e.Source, e, args) :
                                    e.IsGenericParameterization
                                        ? TypeBuilder.Parameterize(e.Source, e.GenericDefinition, args)
                                        : e;
                            }

                            return e.IsGenericDefinition ? m : e;
                        }
                    }
                }

                Log.Warning(m.Source, ErrorCode.I0000, m.Quote() + " was not found");
            }

            if (m.IsGenericParameterization)
            {
                var args = new DataType[m.GenericArguments.Length];

                for (int i = 0; i < args.Length; i++)
                    args[i] = GetType(m.GenericArguments[i]);

                return TypeBuilder.Parameterize(m.Source, m.GenericDefinition, args);
            }

            return m;
        }

        public Method GetMethod(Method m, Expression obj)
        {
            var r = GetMethodInternal(m, obj);
            return r != m ? GetMethod(r, obj) : r;
        }

        public Property GetProperty(Property m, Expression obj)
        {
            if (m == null)
                return null;

            Member result;
            if (SwapMembers.TryGetValue(m, out result))
                return (Property)result;

            var t0 = m.DeclaringType;
            var t1 = GetObjectType(obj, t0);

            if (t0 != t1)
            {
                for (var t = t1; t != null; t = t.Base)
                    foreach (var e in t.Properties)
                        if (e.MasterDefinition == m.MasterDefinition)
                            return e;

                Log.Warning(m.Source, ErrorCode.I0000, m.Quote() + " was not found");
            }

            return m;
        }

        public Event GetEvent(Event m, Expression obj)
        {
            if (m == null)
                return null;

            Member result;
            if (SwapMembers.TryGetValue(m, out result))
                return (Event)result;

            var t0 = m.DeclaringType;
            var t1 = GetObjectType(obj, t0);

            if (t0 != t1)
            {
                for (var t = t1; t != null; t = t.Base)
                    foreach (var e in t.Events)
                        if (e.MasterDefinition == m.MasterDefinition)
                            return e;

                Log.Warning(m.Source, ErrorCode.I0000, m.Quote() + " was not found");
            }

            return m;
        }

        public Function GetFunction(Function m)
        {
            if (m == null)
                return null;

            Member result;
            if (SwapMembers.TryGetValue(m, out result))
                return (Function)result;

            var method = m as Method;
            if (method?.DeclaringMember != null)
            {
                switch (method.DeclaringMember.MemberType)
                {
                    case MemberType.Event:
                    {
                        var resultEvent = GetEvent((Event)method.DeclaringMember, null);

                        if (resultEvent.AddMethod != null && resultEvent.AddMethod.MasterDefinition == m.MasterDefinition)
                            return resultEvent.AddMethod;
                        if (resultEvent.RemoveMethod != null && resultEvent.RemoveMethod.MasterDefinition == m.MasterDefinition)
                            return resultEvent.RemoveMethod;

                        Log.Warning(m.Source, ErrorCode.I0000, m.Quote() + " was not found");
                        break;
                    }
                    case MemberType.Property:
                    {
                        var resultProperty = GetProperty((Property)method.DeclaringMember, null);

                        if (resultProperty.GetMethod != null && resultProperty.GetMethod.MasterDefinition == m.MasterDefinition)
                            return resultProperty.GetMethod;
                        if (resultProperty.SetMethod != null && resultProperty.SetMethod.MasterDefinition == m.MasterDefinition)
                            return resultProperty.SetMethod;

                        Log.Warning(m.Source, ErrorCode.I0000, m.Quote() + " was not found");
                        break;
                    }
                }
            }

            return m;
        }

        public Field GetField(Field m, Expression obj)
        {
            if (m == null)
                return null;

            Member result;
            if (SwapMembers.TryGetValue(m, out result))
                return (Field)result;

            var t0 = m.DeclaringType;
            var t1 = GetObjectType(obj, t0);

            if (t0 != t1)
            {
                for (var t = t1; t != null; t = t.Base)
                {
                    foreach (var e in t.Fields)
                        if (e.MasterDefinition == m.MasterDefinition)
                            return e;
                    foreach (var e in t.Events)
                        if (e.ImplicitField != null && e.ImplicitField.MasterDefinition == m.MasterDefinition)
                            return e.ImplicitField;
                    foreach (var e in t.Properties)
                        if (e.ImplicitField != null && e.ImplicitField.MasterDefinition == m.MasterDefinition)
                            return e.ImplicitField;
                }

                Log.Warning(m.Source, ErrorCode.I0000, m.Quote() + " was not found");
            }

            return m;
        }

        void VisitParameterList(Parameter[] pl)
        {
            foreach (var p in pl)
                p.Type = GetType(p.Type);
        }

        void VisitType(DataType dt)
        {
            if (!Begin(dt))
                return;

            foreach (var it in dt.NestedTypes)
                VisitType(it);

            if (dt.Base != null)
                dt.SetBase(GetType(dt.Base));
            for (int i = 0; i < dt.Interfaces.Length; i++)
                dt.Interfaces[i] = (InterfaceType)GetType(dt.Interfaces[i]);

            if (dt.IsGenericDefinition)
            {
                foreach (var gt in dt.GenericParameters)
                {
                    if (gt.Base != null)
                        gt.SetBase(GetType(gt.Base));
                    for (int i = 0; i < gt.Interfaces.Length; i++)
                        gt.Interfaces[i] = (InterfaceType)GetType(gt.Interfaces[i]);
                }
            }

            if (dt is DelegateType)
            {
                (dt as DelegateType).SetReturnType(GetType(dt.ReturnType));
                VisitParameterList(dt.Parameters);
            }

            foreach (var m in dt.Constructors)
            {
                m.ReturnType = GetType(m.ReturnType);
                VisitParameterList(m.Parameters);
            }

            foreach (var m in dt.Methods)
            {
                m.ReturnType = GetType(m.ReturnType);
                VisitParameterList(m.Parameters);

                if (m.OverriddenMethod != null)
                    m.SetOverriddenMethod(GetMethod(m.OverriddenMethod, null));
                if (m.ImplementedMethod != null)
                    m.SetImplementedMethod(GetMethod(m.ImplementedMethod, null));

                if (m.IsGenericDefinition)
                {
                    foreach (var gt in m.GenericParameters)
                    {
                        if (gt.Base != null)
                            gt.SetBase(GetType(gt.Base));
                        for (int i = 0; i < gt.Interfaces.Length; i++)
                            gt.Interfaces[i] = (InterfaceType)GetType(gt.Interfaces[i]);
                    }

                    foreach (var p in m.GenericParameterizations)
                    {
                        p.ReturnType = GetType(p.ReturnType);
                        VisitParameterList(p.Parameters);

                        if (p.OverriddenMethod != null)
                            p.SetOverriddenMethod(GetMethod(p.OverriddenMethod, null));
                        if (p.ImplementedMethod != null)
                            p.SetImplementedMethod(GetMethod(p.ImplementedMethod, null));
                    }
                }
            }

            foreach (var m in dt.Events)
            {
                m.ReturnType = GetType(m.ReturnType);

                if (m.ImplicitField != null)
                    m.ImplicitField.ReturnType = GetType(m.ImplicitField.ReturnType);
                if (m.AddMethod != null)
                {
                    m.AddMethod.ReturnType = GetType(m.AddMethod.ReturnType);
                    VisitParameterList(m.AddMethod.Parameters);
                }
                if (m.RemoveMethod != null)
                {
                    m.RemoveMethod.ReturnType = GetType(m.RemoveMethod.ReturnType);
                    VisitParameterList(m.RemoveMethod.Parameters);
                }

                if (m.OverriddenEvent != null)
                    m.SetOverriddenEvent(GetEvent(m.OverriddenEvent, null));
                if (m.ImplementedEvent != null)
                    m.SetImplementedEvent(GetEvent(m.ImplementedEvent, null));
            }

            foreach (var m in dt.Properties)
            {
                m.ReturnType = GetType(m.ReturnType);
                VisitParameterList(m.Parameters);

                if (m.ImplicitField != null)
                    m.ImplicitField.ReturnType = GetType(m.ImplicitField.ReturnType);
                if (m.GetMethod != null)
                {
                    m.GetMethod.ReturnType = GetType(m.GetMethod.ReturnType);
                    VisitParameterList(m.GetMethod.Parameters);
                }
                if (m.SetMethod != null)
                {
                    m.SetMethod.ReturnType = GetType(m.SetMethod.ReturnType);
                    VisitParameterList(m.SetMethod.Parameters);
                }

                if (m.OverriddenProperty != null)
                    m.SetOverriddenProperty(GetProperty(m.OverriddenProperty, null));
                if (m.ImplementedProperty != null)
                    m.SetImplementedProperty(GetProperty(m.ImplementedProperty, null));
            }

            foreach (var m in dt.Casts)
            {
                m.ReturnType = GetType(m.ReturnType);
                VisitParameterList(m.Parameters);
            }
            foreach (var m in dt.Operators)
            {
                m.ReturnType = GetType(m.ReturnType);
                VisitParameterList(m.Parameters);
            }

            foreach (var m in dt.Fields)
                m.ReturnType = GetType(m.ReturnType);
            foreach (var m in dt.Literals)
                m.ReturnType = GetType(m.ReturnType);

            if (dt.IsGenericDefinition)
                for (int i = 0; i < dt.GenericParameterizations.Count; i++)
                    VisitType(dt.GenericParameterizations[i]);
        }

        void VisitNamespace(Namespace root)
        {
            foreach (var ns in root.Namespaces)
                VisitNamespace(ns);
            foreach (var dt in root.Types)
                VisitType(dt);
        }

        public override bool Begin()
        {
            VisitNamespace(Data.IL);
            return true;
        }

        public override void End(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.VariableDeclaration:
                {
                    var s = e as VariableDeclaration;
                    for (var var = s.Variable; var != null; var = var.Next)
                        var.ValueType = GetType(var.ValueType);
                    break;
                }
            }
        }

        public override void End(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.This:
                {
                    var s = e as This;
                    s.ThisType = GetType(s.ThisType);
                    break;
                }
                case ExpressionType.Base:
                {
                    var s = e as Base;
                    s.BaseType = GetType(s.BaseType);
                    break;
                }
                case ExpressionType.Constant:
                {
                    var s = e as Constant;
                    s.ValueType = GetType(s.ValueType);
                    break;
                }
                case ExpressionType.Default:
                {
                    var s = e as Default;
                    s.ValueType = GetType(s.ValueType);
                    break;
                }
                case ExpressionType.GetProperty:
                {
                    var s = e as GetProperty;
                    s.Property = GetProperty(s.Property, s.Object);
                    break;
                }
                case ExpressionType.SetProperty:
                {
                    var s = e as SetProperty;
                    s.Property = GetProperty(s.Property, s.Object);
                    break;
                }
                case ExpressionType.LoadField:
                {
                    var s = e as LoadField;
                    s.Field = GetField(s.Field, s.Object);
                    break;
                }
                case ExpressionType.StoreField:
                {
                    var s = e as StoreField;
                    s.Field = GetField(s.Field, s.Object);
                    break;
                }
                case ExpressionType.LoadArgument:
                {
                    var s = e as LoadArgument;
                    s.Function = s.Function.Match(
                        fun => new Either<Function, Lambda>(GetFunction(fun)),
                        lam => s.Function);
                    break;
                }
                case ExpressionType.StoreArgument:
                {
                    var s = e as StoreArgument;
                    s.Function = s.Function.Match(
                        fun => new Either<Function, Lambda>(GetFunction(fun)),
                        lam => s.Function);
                    break;
                }
                case ExpressionType.CallMethod:
                {
                    var s = e as CallMethod;
                    s.Method = GetMethod(s.Method, s.Object);
                    break;
                }
                case ExpressionType.CallConstructor:
                {
                    var s = e as CallConstructor;
                    s.Constructor = GetConstructor(s.Constructor);
                    break;
                }
                case ExpressionType.CallCast:
                {
                    var s = e as CallCast;
                    s.Cast = GetCast(s.Cast);
                    break;
                }
                case ExpressionType.CallBinOp:
                {
                    var s = e as CallBinOp;
                    s.Operator = GetOperator(s.Operator);
                    break;
                }
                case ExpressionType.CallUnOp:
                {
                    var s = e as CallUnOp;
                    s.Operator = GetOperator(s.Operator);
                    break;
                }
                case ExpressionType.NewObject:
                {
                    var s = e as NewObject;
                    s.Constructor = GetConstructor(s.Constructor);
                    break;
                }
                case ExpressionType.NewArray:
                {
                    var s = e as NewArray;
                    s.ArrayType = (RefArrayType)GetType(s.ArrayType);
                    break;
                }
                case ExpressionType.NewDelegate:
                {
                    var s = e as NewDelegate;
                    s.DelegateType = (DelegateType)GetType(s.DelegateType);
                    s.Method = GetMethod(s.Method, s.Object);
                    break;
                }
                case ExpressionType.CastOp:
                {
                    var s = e as CastOp;
                    s.TargetType = GetType(s.TargetType);
                    break;
                }
                case ExpressionType.IsOp:
                {
                    var s = e as IsOp;
                    s.TestType = GetType(s.TestType);
                    break;
                }
                case ExpressionType.AsOp:
                {
                    var s = e as AsOp;
                    s.TestType = GetType(s.TestType);
                    break;
                }
                case ExpressionType.AddListener:
                {
                    var s = e as AddListener;
                    s.Event = GetEvent(s.Event, s.Object);
                    break;
                }
                case ExpressionType.RemoveListener:
                {
                    var s = e as RemoveListener;
                    s.Event = GetEvent(s.Event, s.Object);
                    break;
                }
                case ExpressionType.TypeOf:
                {
                    var s = e as TypeOf;
                    s.Type = GetType(s.Type);
                    break;
                }
            }
        }
    }
}
