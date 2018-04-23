using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.CPlusPlus
{
    public class CppType
    {
        internal bool _isOptimized;

        public readonly bool IsOpaque;
        public readonly bool EmitTypeObject;
        public readonly bool EmitTypeStruct;
        public readonly bool EmitTypeOfDeclaration;
        public readonly int InheritedFieldCount;
        public readonly string TypeOfType;
        public readonly string TypeOfFunction;
        public readonly string ReflectedName;
        public readonly string StructName;
        public readonly List<Field> Fields = new List<Field>();
        public readonly List<Field> FlattenedFields = new List<Field>();
        public readonly List<Field> ReflectedFields = new List<Field>();
        public readonly List<Function> Functions = new List<Function>();
        public readonly List<Function> ReflectedFunctions = new List<Function>();
        public readonly List<Function> StaticMethods = new List<Function>();
        public readonly List<CppType> MethodTypes = new List<CppType>();
        public readonly List<Method> InstanceMethods = new List<Method>();
        public readonly List<Method> VTable = new List<Method>();
        public readonly List<DataType> PrecalcedTypes = new List<DataType>();
        public readonly List<DataType> Dependencies = new List<DataType>();
        public readonly HashSet<string> StringConsts = new HashSet<string>();
        public readonly HashSet<DataType> TypeObjects = new HashSet<DataType>();
        public Declarations Declarations = Declarations.Empty;
        public int MethodIndex = -1;
        public int MethodRank = 0;

        public ParameterFlags ParameterFlags => IsOpaque ? ParameterFlags.ObjectByRef : 0;

        internal CppType(
            IEnvironment env,
            CppBackend backend,
            DataType dt)
        {
            EmitTypeObject = !env.HasProperty(dt, "TypeOfFunction");
            IsOpaque = backend.IsOpaque(dt);
            ReflectedName = dt.IsIntrinsic ? dt.QualifiedName : dt.ToString();
            StructName = backend.GetStaticName(dt, dt);
            TypeOfFunction = StructName + "_typeof";
            TypeOfType = StructName + "_type";

            if (dt.IsGenericMethodType)
            {
                TypeOfType = "uClassType";
                return;
            }

            EmitTypeOfDeclaration = true;

            switch (dt.TypeType)
            {
                case TypeType.Class:
                case TypeType.Struct:
                {
                    foreach (var m in dt.Methods)
                        AddClassMethod(env, m);
                    foreach (var m in dt.Properties)
                    {
                        AddClassMethod(env, m.GetMethod);
                        AddClassMethod(env, m.SetMethod);
                    }
                    foreach (var m in dt.Events)
                    {
                        AddClassMethod(env, m.AddMethod);
                        AddClassMethod(env, m.RemoveMethod);
                    }

                    foreach (var f in Functions)
                    {
                        if (f.IsConstructor ||
                            !f.IsStatic && !f.IsVirtualBase ||
                            f.IsVirtual && (
                                backend.IsTemplate(f.DeclaringType) ||
                                f.DeclaringType.Base == null
                            ))
                            continue;

                        StaticMethods.Add(f);
                    }

                    Fields.AddRange(dt.EnumerateFields());

                    for (var bt = dt.Base; bt != null; bt = bt.Base)
                        foreach (var f in bt.EnumerateFields())
                            if (!f.IsStatic)
                                FlattenedFields.Add(f);
                    InheritedFieldCount = FlattenedFields.Count;

                    foreach (var f in Fields)
                        if (!f.IsStatic)
                            FlattenedFields.Add(f);
                    // Strategically add static fields last
                    foreach (var f in Fields)
                        if (f.IsStatic)
                            FlattenedFields.Add(f);

                    // Early out unless reflection is enabled
                    if (!backend.EnableReflection || (!backend.EnableDebugDumps && !dt.IsPublic))
                        break;

                    foreach (var f in dt.EnumerateFunctions())
                    {
                        var prototype = f.Prototype;

                        if (!prototype.IsPublic || env.GetBool(f, "IsIntrinsic"))
                            continue;
                        switch (prototype.MemberType)
                        {
                            case MemberType.Cast:
                            case MemberType.Operator:
                            case MemberType.Finalizer:
                                continue;

                            case MemberType.Constructor:
                            {
                                if (prototype.IsStatic || f.ReturnType.IsVoid)
                                    continue;

                                break;
                            }
                            case MemberType.Method:
                            {
                                var method = (Method)prototype;
                                if (method.ImplementedMethod != null ||
                                    method.OverriddenMethod != null ||
                                    method.IsGenerated && method.DeclaringMember == null)
                                    continue;

                                break;
                            }
                        }

                        ReflectedFunctions.Add(f);
                    }

                    foreach (var f in backend.EnableDebugDumps ? dt.EnumerateFields() : dt.Fields)
                    {
                        var prototype = f.Prototype;

                        if (!backend.EnableDebugDumps && (!prototype.IsPublic || prototype.IsGenerated))
                            continue;

                        ReflectedFields.Add(f);
                    }

                    break;
                }
                case TypeType.Interface:
                {
                    foreach (var m in dt.Methods)
                        AddInterfaceMethod(m);
                    foreach (var m in dt.Properties)
                    {
                        AddInterfaceMethod(m.GetMethod);
                        AddInterfaceMethod(m.SetMethod);
                    }
                    foreach (var m in dt.Events)
                    {
                        AddInterfaceMethod(m.AddMethod);
                        AddInterfaceMethod(m.RemoveMethod);
                    }

                    // Early out unless reflection is enabled
                    if (!backend.EnableReflection || !dt.IsPublic)
                        break;

                    ReflectedFunctions.AddRange(dt.EnumerateFunctions());
                    break;
                }
            }

            // Clear fields on structs where TypeName is overridden,
            // to avoid invalid offsetof() in generated code.
            if (dt.IsStruct && env.HasProperty(dt, "TypeName"))
            {
                FlattenedFields.Clear();
                ReflectedFields.Clear();
            }

            EmitTypeStruct = EmitTypeObject && (
                    VTable.Any(x => x.OverriddenMethod == null) ||
                    dt.Base == null && dt.Interfaces.Length > 0 ||
                    dt.Base != null && dt.Interfaces.Length > dt.Base.Interfaces.Length
                );

            if (!EmitTypeStruct)
                TypeOfType = dt.IsValueType
                    ? "uStructType"
                    : backend.GetTypeOfType(dt.Base, dt);

            Functions.Sort();
            ReflectedFields.Sort();
            ReflectedFunctions.Sort();
            InstanceMethods.Sort();
            VTable.Sort();
        }

        void AddFunction(IEnvironment env, Function f)
        {
            if (f == null || f.IsAbstract ||
                env.GetBool(f, "IsIntrinsic"))
                return;

            Functions.Add(f);
        }

        void AddClassMethod(IEnvironment env, Method m)
        {
            AddFunction(env, m);

            if (m == null || m.IsStatic ||
                m.ImplementedMethod != null ||
                env.GetBool(m, "IsIntrinsic"))
                return;

            if (m.IsVirtual)
                VTable.Add(m);

            if (m.OverriddenMethod != null && !m.DeclaringType.IsStruct)
                return;

            InstanceMethods.Add(m);
        }

        void AddInterfaceMethod(Method m)
        {
            if (m == null)
                return;

            InstanceMethods.Add(m);
            VTable.Add(m);
        }
    }
}
