using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Backends.CPlusPlus
{
    class CppPrecalc : Pass
    {
        readonly BodyFlags _flags = BodyFlags.ClassMember;
        readonly CppBackend _backend;
        CppType _type;
        CppFunction _func;

        public CppPrecalc(CppBackend backend)
            : base(backend)
        {
            _backend = backend;
            _type = _backend.GetType(DataType.Null);
            _func = _backend.GetFunction(Function.Null);
        }

        public override bool Begin(DataType dt)
        {
            _type = _backend.GetType(dt);
            return true;
        }

        public override void End(DataType dt)
        {
            _type = _backend.GetType(DataType.Null);
        }

        public override bool Begin(Function f)
        {
            _func = _backend.GetFunction(f);
            return true;
        }

        public override void End(Function f)
        {
            _func = _backend.GetFunction(Function.Null);
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.Constant:
                {
                    var str = e.ConstantValue as string;
                    if (str != null)
                        _type.StringConsts.Add((string)e.ConstantValue);
                    break;
                }
                case ExpressionType.LoadField:
                case ExpressionType.StoreField:
                {
                    var s = (FieldExpression) e;
                    var field = s.Field;
                    var obj = s.Object;
                    if (field.IsStatic)
                        RegisterDependency(field.DeclaringType);

                    if (_backend.IsConstrained(field))
                    {
                        if (!(obj is AddressOf && _backend.IsConstrained(obj.ReturnType) && obj.ActualValue.HasStorage()) &&
                                !(!field.IsStatic && field.DeclaringType.IsReferenceType))
                            RegisterType(field.DeclaringType);
                    }
                    else
                    {
                        if (!_backend.IsTemplate(field.DeclaringType) && _backend.IsConstrained(field.MasterDefinition.ReturnType) ||
                                field.IsStatic && !field.DeclaringType.MasterDefinition.IsClosed)
                            RegisterType(field.IsStatic, field.DeclaringType);
                    }
                    break;
                }
                case ExpressionType.LoadArgument:
                {
                    var s = (LoadArgument) e;
                    if (!s.Parameter.IsReference && (
                            s.Parameter.Type.IsReferenceType ||
                            _flags.HasFlag(BodyFlags.ClassMember) ||
                            _backend.IsConstrained(s.Parameter.Type)))
                        RegisterType(u == ExpressionUsage.VarArg && s.Parameter.IsReference &&
                                _backend.IsConstrained(s.Parameter.Type),
                            s.Parameter.Type);
                    break;
                }
                case ExpressionType.StoreArgument:
                {
                    var s = (StoreArgument) e;
                    if (s.Parameter.IsReference &&
                            _backend.IsConstrained(s.Parameter.Type) &&
                            !_flags.HasFlag(BodyFlags.ClassMember))
                        RegisterType(s.Value is Default ||
                                !s.Value.HasStorage(),
                            s.Parameter.Type);
                    break;
                }
                case ExpressionType.Base:
                    RegisterType(Function != null && Type.IsValueType, e.ReturnType);
                    break;
                case ExpressionType.AllocObject:
                    RegisterType(((AllocObject) e).ObjectType);
                    break;
                case ExpressionType.NewObject:
                    RegisterType(((NewObject) e).Constructor.DeclaringType);
                    break;
                case ExpressionType.NewDelegate:
                {
                    var s = (NewDelegate) e;
                    RegisterType(s.ReturnType);
                    RegisterInterface(s.Method);

                    if (_backend.HasTypeParameter(s.Method))
                        RegisterType(s.Method.GenericType ?? s.Method.DeclaringType);
                    break;
                }
                case ExpressionType.TypeOf:
                    RegisterType(((TypeOf) e).TypeType);
                    break;
                case ExpressionType.AddListener:
                case ExpressionType.RemoveListener:
                case ExpressionType.SetProperty:
                case ExpressionType.GetProperty:
                case ExpressionType.CallMethod:
                {
                    var s = (CallExpression) e;
                    var func = s.Function;
                    RegisterInterface(func);

                    if (func.IsGenericMethod)
                        RegisterType(func.GenericType);
                    else if (_backend.HasTypeParameter(func))
                        RegisterType(func.DeclaringType);

                    if (func.IsStatic && !func.Prototype.IsConstructor)
                        RegisterDependency(func.GenericType ?? func.DeclaringType);
                    break;
                }
                case ExpressionType.NewArray:
                    RegisterType(((NewArray) e).ArrayType);
                    break;
                case ExpressionType.CastOp:
                    if (!e.ReturnType.IsInterface && 
                            ((CastOp) e).CastType == CastType.Down)
                        RegisterType(e.ReturnType);
                    break;
                case ExpressionType.IsOp:
                    RegisterType(((IsOp) e).TestType);
                    break;
                case ExpressionType.AsOp:
                    RegisterType(((AsOp) e).TestType);
                    break;
            }
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.VariableDeclaration:
                {
                    var s = (VariableDeclaration) e;
                    if (_backend.IsConstrained(s.Variable.ValueType))
                    {
                        for (var var = s.Variable; var != null; var = var.Next)
                            _func.Constrained.Add(var);
                        RegisterType(s.Variable.ValueType);
                    }
                    break;
                }
                case StatementType.TryCatchFinally:
                {
                    var s = (TryCatchFinally) e;
                    foreach (var c in s.CatchBlocks)
                        RegisterType(c.Exception.ValueType);
                    break;
                }
                case StatementType.Return:
                {
                    var value = ((Return) e).Value;
                        RegisterType(!Function.ReturnType.IsVoid &&
                            !_flags.HasFlag(BodyFlags.ClassMember) &&
                            _backend.IsConstrained(Function.ReturnType) &&
                            !value.HasStorage(),
                        Function.ReturnType);
                    break;
                }
            }
        }

        public override void End()
        {
            foreach (var ns in Data.IL.EnumerateNamespacesBfs())
            {
                foreach (var dt in ns.EnumerateDataTypesBfs())
                {
                    var type = _backend.GetType(dt);
                    var dependencies = new HashSet<DataType>();
                    var precalc = new HashSet<DataType>();

                    foreach (var f in dt.EnumerateFunctions())
                    {
                        var func = _backend.GetFunction(f);

                        if (!f.IsGenericMethod)
                        {
                            if (f != dt.Initializer)
                                dependencies.AddRange(func.Dependencies);
                            precalc.AddRange(func.PrecalcedTypes);
                        }
                        else
                        {
                            var gt = f.GenericType;
                            var gtype = _backend.GetType(gt);

                            foreach (var t in func.PrecalcedTypes)
                                func.Dependencies.Remove(t);

                            gtype.Dependencies.AddRange(func.Dependencies);
                            gtype.PrecalcedTypes.AddRange(func.PrecalcedTypes);
                            gtype.MethodIndex = type.MethodTypes.Count;
                            gtype.MethodRank = gt.GenericParameters.Length;
                            type.MethodTypes.Add(gtype);
                        }
                    }

                    foreach (var m in type.MethodTypes)
                    {
                        foreach (var t in dependencies)
                            m.Dependencies.Remove(t);
                        foreach (var t in precalc)
                            m.Dependencies.Remove(t);
                    }

                    type.Dependencies.AddRange(dependencies);
                    type.Dependencies.Sort();
                    type.PrecalcedTypes.AddRange(precalc);
                    type.PrecalcedTypes.Sort();
                    type.TypeObjects.Remove(dt);
                    type.Declarations = _backend.IncludeResolver.GetDeclarations(dt, type);
                }
            }

            foreach (var ns in Data.IL.EnumerateNamespacesBfs())
                foreach (var dt in ns.EnumerateDataTypesBfs())
                    OptimizeDependencies(_backend.GetType(dt));
        }

        bool OptimizeDependencies(CppType type)
        {
            if (!type._isOptimized)
            {
                type._isOptimized = true;

                foreach (var m in type.MethodTypes)
                    OptimizeDependencies(m);

                for (int i = type.Dependencies.Count - 1; i >= 0; i--)
                    if (type.Dependencies[i].Initializer == null &&
                        OptimizeDependencies(_backend.GetType(type.Dependencies[i])))
                        type.Dependencies.RemoveAt(i);
            }

            return type.Dependencies.Count == 0;
        }

        void RegisterInterface(Member member)
        {
            RegisterType(member.DeclaringType.IsInterface, member.DeclaringType);
        }

        void RegisterType(bool cond, DataType dt)
        {
            if (cond)
                RegisterType(dt);
        }

        void RegisterType(DataType dt)
        {
            if (dt.IsClosed)
                _type.TypeObjects.Add(dt);
            else if (dt.IsGenericParameter)
                _type.TypeObjects.Add(Type);
            else if (dt.IsGenericMethodType)
                RegisterType(dt.ParentType.MasterDefinition);
            else if (dt.MasterDefinition != Type && !dt.IsArray)
                _type.TypeObjects.Add(dt.MasterDefinition);

            if (!_type.TypeObjects.Contains(dt) &&
                !(Function.IsGenericMethod && CppBackend.Compare(dt, Function.GenericType)) &&
                !CppBackend.Compare(dt, Type) &&
                !dt.IsGenericParameter &&
                dt.MasterDefinition.HasRefCount)
                _func.PrecalcedTypes.Add(dt);
        }

        void RegisterDependency(DataType dt)
        {
            if (!(Function.IsGenericMethod && CppBackend.Compare(dt, Function.GenericType)) &&
                !CppBackend.Compare(dt, Type) && dt.HasInitializer &&
                dt.BuiltinType != BuiltinType.String && dt.BuiltinType != BuiltinType.Char &&
                dt.MasterDefinition.HasRefCount)
                _func.Dependencies.Add(dt);
        }
    }
}
