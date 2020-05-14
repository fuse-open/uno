using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Logging;
using Type = IKVM.Reflection.Type;

namespace Uno.Compiler.Backends.CIL
{
    public class CilLinker : LogObject
    {
        const BindingFlags _ctorFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        const BindingFlags _memberFlags = _ctorFlags | BindingFlags.Static;
        static readonly Binder _binder = Type.DefaultBinder;

        public readonly Universe Universe = new Universe(UniverseOptions.MetadataOnly);

        readonly IEssentials _essentials;
        readonly HashSet<Assembly> _copyAssemblies = new HashSet<Assembly>();
        readonly Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();
        readonly Dictionary<Field, FieldInfo> _fields = new Dictionary<Field, FieldInfo>();
        readonly Dictionary<Function, MethodInfo> _methods = new Dictionary<Function, MethodInfo>();
        readonly Dictionary<Constructor, ConstructorInfo> _constructors = new Dictionary<Constructor, ConstructorInfo>();
        readonly Dictionary<DataType, ConstructorInfo> _typeConstructors = new Dictionary<DataType, ConstructorInfo>();
        readonly Dictionary<DataType, MethodInfo> _typeMethods = new Dictionary<DataType, MethodInfo>();
        readonly Dictionary<DataType, Type> _types = new Dictionary<DataType, Type>();
        readonly bool _isReferenceAssembly;

        public IEnumerable<Assembly> CopyAssemblies => _copyAssemblies;
        public IReadOnlyDictionary<DataType, Type> TypeMap => _types;

        public readonly Type System_Object;
        public readonly Type System_String;
        public readonly Type System_ValueType;
        public readonly Type System_MulticastDelegate;
        public readonly Type System_Enum;
        public readonly Type System_IntPtr;
        public readonly Type System_Int32;
        public readonly Type System_Void;
        public readonly ConstructorInfo System_ParamAttribute_ctor;
        public readonly ConstructorInfo System_Diagnostics_DebuggableAttribute_ctor;
        public readonly ConstructorInfo System_Runtime_CompilerServices_ExtensionAttribute_ctor;
        public readonly ConstructorInfo System_Runtime_CompilerServices_InternalsVisibleToAttribute_ctor;
        public readonly ConstructorInfo System_NotSupportedException_ctor;
        public readonly MethodInfo System_Activator_CreateInstance;
        public readonly MethodInfo System_Type_GetTypeFromHandle;

        public CilLinker(Log log, IEssentials essentials, bool isReferenceAssembly = false)
            : base(log)
        {
            _essentials = essentials;
            _isReferenceAssembly = isReferenceAssembly;
            System_Object = Universe.Import(typeof(object));
            System_String = Universe.Import(typeof(string));
            System_ValueType = Universe.Import(typeof(ValueType));
            System_MulticastDelegate = Universe.Import(typeof(MulticastDelegate));
            System_Enum = Universe.Import(typeof(Enum));
            System_IntPtr = Universe.Import(typeof(IntPtr));
            System_Int32 = Universe.Import(typeof(int));
            System_Void = Universe.Import(typeof(void));
            System_ParamAttribute_ctor = Universe.Import(typeof(ParamArrayAttribute)).GetConstructor(Type.EmptyTypes);
            System_Diagnostics_DebuggableAttribute_ctor = Universe.Import(typeof(DebuggableAttribute)).GetConstructor(new[] { Universe.Import(typeof(DebuggableAttribute.DebuggingModes)) });
            System_Runtime_CompilerServices_ExtensionAttribute_ctor = Universe.Import(typeof(ExtensionAttribute)).GetConstructor(Type.EmptyTypes);
            System_Runtime_CompilerServices_InternalsVisibleToAttribute_ctor = Universe.Import(typeof(InternalsVisibleToAttribute)).GetConstructor(new[] {System_String});
            System_NotSupportedException_ctor = Universe.Import(typeof(NotSupportedException)).GetConstructor(new[] {System_String});
            System_Activator_CreateInstance = Universe.Import(typeof(Activator)).GetMethod("CreateInstance", Type.EmptyTypes);
            System_Type_GetTypeFromHandle = Universe.Import(typeof(System.Type)).GetMethod("GetTypeFromHandle");
            _types.Add(DataType.Void, System_Void);
        }

        public Assembly AddAssemblyFile(string filename, bool copyToOutputDir = false)
        {
            var asm = Universe.LoadFile(filename);
            _assemblies[asm.FullName] = asm;

            if (copyToOutputDir)
                _copyAssemblies.Add(asm);

            return asm;
        }

        public void AddAssembly(string partialName)
        {
            // LoadWithPartialName() generates a warning because the result will be affected when specified assembly is upgraded in GAC.
            // However, for convenience, we want to be able to load i.e. "System.Core" without having to specify the full strong name.
#pragma warning disable 0618
            var partial = System.Reflection.Assembly.LoadWithPartialName(partialName);
#pragma warning restore 0618

            if (partial == null)
                throw new Exception("Not found in GAC. Please check the name.");

            var asm = Universe.Load(partial.FullName);
            _assemblies[asm.FullName] = asm;
        }

        public void AddType(DataType key, Type value)
        {
            _types.Add(key, value);
        }

        public void AddField(Field field, FieldInfo info)
        {
            _fields.Add(field, info);
        }

        public void AddMethod(Function function, MethodInfo info)
        {
            _methods.Add(function, info);
        }

        public void AddConstructor(Constructor ctor, ConstructorInfo info)
        {
            _constructors.Add(ctor, info);
        }

        public void AddMethod(DataType type, MethodInfo info)
        {
            _typeMethods.Add(type, info);
        }

        public void AddConstructor(DataType type, ConstructorInfo info)
        {
            _typeConstructors.Add(type, info);
        }

        public Assembly GetAssembly(string name)
        {
            Assembly result;
            if (_assemblies.TryGetValue(name, out result))
                return result;

            throw new FatalException(Source.Unknown, ErrorCode.E0000, ".NET assembly not resolved: " + name);
        }

        public Type TryGetType(string name)
        {
            if (name == null)
                return null;

            foreach (var asm in _assemblies.Values)
            {
                var type = asm.GetType(name, false);
                if (type != null)
                    return type;
            }

            try
            {
                return Universe.Import(System.Type.GetType(name));
            }
            catch
            {
                return null;
            }
        }

        public Type GetType(DataType dt)
        {
            Type result;
            if (_types.TryGetValue(dt, out result))
                return result;

            switch (dt.TypeType)
            {
                case TypeType.RefArray:
                    result = GetType(dt.ElementType).MakeArrayType();
                    break;

                case TypeType.Enum:
                    if (!dt.IsMasterDefinition)
                        result = GetType(dt.MasterDefinition);
                    else if (dt.IsNestedType)
                        result = GetType(dt.ParentType).GetNestedType(dt.CilTypeName(), BindingFlags.Public | BindingFlags.NonPublic);

                    break;

                case TypeType.Class:
                case TypeType.Struct:
                case TypeType.Interface:
                case TypeType.Delegate:
                    if (dt.IsFlattenedParameterization)
                    {
                        var fargs = dt.FlattenedArguments;
                        var pargs = new Type[fargs.Length];

                        for (int i = 0; i < pargs.Length; i++)
                            pargs[i] = GetType(fargs[i]);

                        if (!dt.IsMasterDefinition) // Should always be true
                            result = GetType(dt.MasterDefinition).MakeGenericType(pargs);
                    }

                    if (result == null && dt.IsNestedType)
                        result = GetType(dt.ParentType).GetNestedType(dt.CilTypeName(), BindingFlags.Public | BindingFlags.NonPublic);

                    break;

                case TypeType.GenericParameter:
                {
                    var pt = (GenericParameterType)dt;

                    if (pt.IsGenericTypeParameter)
                    {
                        var gt = pt.GenericTypeParent;
                        var owner = GetType(gt);

                        var gargs = owner.GetGenericArguments();
                        var pargs = gt.FlattenedParameters;

                        for (int i = 0, l = gargs.Length; i < l; i++)
                        {
                            if (pargs[i] == pt)
                                result = gargs[i];

                            _types[pargs[i]] = gargs[i];
                        }

                        if (result != null)
                            return result;
                    }

                    break;
                }
            }

            if (result == null)
            {
                var asm = dt.Package.Tag as Assembly;

                if (dt.HasAttribute(_essentials.DotNetTypeAttribute) && !_isReferenceAssembly)
                    result = TryGetType(dt.TryGetAttributeString(_essentials.DotNetTypeAttribute) ?? dt.CilTypeName());
                else if (asm != null)
                    result = asm.GetType(dt.CilTypeName());

                if (result == null)
                    throw new FatalException(dt.Source, ErrorCode.E0000, ".NET type not resolved: " + dt + " [flags: " + dt.Stats.ToString().ToLower() + "]");
            }

            _types.Add(dt, result);
            return result;
        }

        public Type[] GetParameterTypes(Parameter[] list)
        {
            var result = new Type[list.Length];

            for (int i = 0, l = list.Length; i < l; i++)
            {
                var p = list[i];
                result[i] = GetType(p.Type);

                if (p.IsReference)
                    result[i] = result[i].MakeByRefType();
            }

            return result;
        }

        bool ParameterTypesEquals(ParameterInfo[] a, Type[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
                if (a[i].ParameterType != b[i])
                    return false;

            return true;
        }

        void ThrowPrebuiltSignatureException(ArgumentException e, Entity entity, Parameter[] paramList, Type[] paramTypes)
        {
            for (int i = 0; i < paramTypes.Length; i++)
                if (paramTypes[i].IsTypeBuilder())
                    Log.Error(entity.Source, ErrorCode.E0000, ".NET parameter #" + (i + 1) + " is not a prebuilt type: " + paramList[i].Type);

            throw new FatalException(entity.Source, ErrorCode.E0000, ".NET prebuilt signature error: " + entity, e);
        }

        public MethodInfo GetMethod(Function f)
        {
            MethodInfo result;
            if (_methods.TryGetValue(f, out result))
                return result;

            var method = f as Method;

            if (method != null && method.IsGenericParameterization)
            {
                var md = GetMethod(method.GenericDefinition);

                var targs = new Type[method.GenericArguments.Length];
                for (int i = 0; i < targs.Length; i++)
                    targs[i] = GetType(method.GenericArguments[i]);

                result = md.MakeGenericMethod(targs);

                _methods.Add(f, result);
                return result;
            }

            var type = GetType(f.DeclaringType);

            if (type.IsTypeBuilder())
            {
                if (f != f.MasterDefinition) // Should always be true
                    result = TypeBuilder.GetMethod(type, GetMethod(f.MasterDefinition));
            }
            else if (f is Cast)
            {
                var returnType = GetType(f.ReturnType);
                var operandType = GetType(f.Parameters[0].Type);

                foreach (var mi in type.GetMethods(_memberFlags))
                {
                    var paramList = mi.GetParameters();

                    if (mi.Name != "op_Explicit" && mi.Name != "op_Implicit" ||
                        paramList.Length != 1 ||
                        paramList[0].ParameterType != operandType ||
                        mi.ReturnType != returnType)
                        continue;

                    result = mi;
                    break;
                }
            }
            else if (method != null && method.IsGenericDefinition)
            {
                foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                {
                    if (!mi.IsGenericMethodDefinition ||
                        mi.Name != method.UnoName)
                        continue;

                    var paramList = mi.GetParameters();
                    if (paramList.Length != method.Parameters.Length)
                        continue;

                    var typeParams = mi.GetGenericMethodDefinition().GetGenericArguments();
                    if (typeParams.Length != method.GenericParameters.Length)
                        continue;

                    // TODO: Avoid copying types
                    var oldTypes = new Dictionary<DataType, Type>();
                    foreach (var e in _types)
                        oldTypes.Add(e.Key, e.Value);

                    for (int i = 0; i < typeParams.Length; i++)
                        _types[method.GenericParameters[i]] = typeParams[i];

                    if (GetType(method.ReturnType) == mi.ReturnType && ParameterTypesEquals(mi.GetParameters(), GetParameterTypes(method.Parameters)))
                    {
                        result = mi;
                        break;
                    }

                    // Not found - reset types
                    _types.Clear();
                    foreach (var e in oldTypes)
                        _types.Add(e.Key, e.Value);
                }
            }
            else
            {
                var name = f.UnoName;

                if (type == System_String)
                    switch (name)
                    {
                        case "get_Item":
                            name = "get_Chars";
                            break;
                        case "op_Addition":
                            name = "Concat";
                            break;
                    }

                var paramTypes = GetParameterTypes(f.Parameters);

                try
                {
                    result = type.GetMethod(name, _memberFlags, _binder, paramTypes, null);

                    if (result == null && method != null)
                    {
                        if (method.ImplementedMethod != null)
                            TryGetImplementedMethod(type, method.ImplementedMethod.Name, paramTypes, out result);
                        else
                            TryGetInterfaceMethod(type, name, paramTypes, out result);
                    }
                }
                catch (ArgumentException e)
                {
                    ThrowPrebuiltSignatureException(e, f, f.Parameters, paramTypes);
                }
            }

            if (result == null)
                throw new FatalException(f.Source, ErrorCode.E0000, ".NET method not resolved: " + f + " [flags: " + f.Stats.ToString().ToLower() + "]");

            _methods.Add(f, result);
            return result;
        }

        public ConstructorInfo GetConstructor(Constructor f)
        {
            ConstructorInfo result;
            if (_constructors.TryGetValue(f, out result))
                return result;

            var type = GetType(f.DeclaringType);

            if (type.IsTypeBuilder())
            {
                if (f != f.MasterDefinition) // Should always be true
                    result = TypeBuilder.GetConstructor(type, GetConstructor((Constructor)f.MasterDefinition));
            }
            else
            {
                var paramTypes = GetParameterTypes(f.Parameters);

                try
                {
                    result = type.GetConstructor(_ctorFlags, _binder, paramTypes, null);
                }
                catch (ArgumentException e)
                {
                    ThrowPrebuiltSignatureException(e, f, f.Parameters, paramTypes);
                }
            }

            if (result == null)
                throw new FatalException(f.Source, ErrorCode.E0000, ".NET constructor not resolved: " + f + " [flags: " + f.Stats.ToString().ToLower() + "]");

            _constructors.Add(f, result);
            return result;
        }

        public FieldInfo GetField(Field f)
        {
            FieldInfo result;
            if (_fields.TryGetValue(f, out result))
                return result;

            var type = GetType(f.DeclaringType);

            if (type.IsTypeBuilder())
            {
                if (!f.IsMasterDefinition) // Should always be true
                    result = TypeBuilder.GetField(type, GetField(f.MasterDefinition));
            }
            else
                result = type.GetField(f.Name, _memberFlags);

            if (result == null)
                throw new FatalException(f.Source, ErrorCode.E0000, ".NET field not resolved: " + f + " [flags: " + f.Stats.ToString().ToLower() + "]");

            _fields.Add(f, result);
            return result;
        }

        public ConstructorInfo GetDelegateConstructor(DelegateType dt)
        {
            ConstructorInfo result;
            if (_typeConstructors.TryGetValue(dt, out result))
                return result;

            var type = GetType(dt);

            if (type.IsTypeBuilder())
            {
                if (!dt.IsMasterDefinition) // Should always be true
                    result = TypeBuilder.GetConstructor(type, GetDelegateConstructor((DelegateType)dt.MasterDefinition));
            }
            else
            {
                result = type.GetConstructor(new[] {System_Object, System_IntPtr});
            }

            if (result == null)
                throw new FatalException(dt.Source, ErrorCode.E0000, ".NET delegate constructor not resolved: " + dt + " [flags: " + dt.Stats.ToString().ToLower() + "]");

            _typeConstructors.Add(dt, result);
            return result;
        }

        public MethodInfo GetDelegateInvokeMethod(DelegateType dt)
        {
            MethodInfo result;
            if (_typeMethods.TryGetValue(dt, out result))
                return result;

            var type = GetType(dt);

            if (type.IsTypeBuilder())
            {
                if (!dt.IsMasterDefinition) // Should always be true
                    result = TypeBuilder.GetMethod(type, GetDelegateInvokeMethod((DelegateType)dt.MasterDefinition));
            }
            else
            {
                try
                {
                    result = type.GetMethod("Invoke");
                }
                catch (ArgumentException e)
                {
                    ThrowPrebuiltSignatureException(e, dt, dt.Parameters, GetParameterTypes(dt.Parameters));
                }
            }

            if (result == null)
                throw new FatalException(dt.Source, ErrorCode.E0000, ".NET delegate invoke method not resolved: " + dt + " [flags: " + dt.Stats.ToString().ToLower() + "]");

            _typeMethods.Add(dt, result);
            return result;
        }

        bool TryGetImplementedMethod(Type type, string name, Type[] paramTypes, out MethodInfo result)
        {
            var dotName = "." + name;

            foreach (var m in type.GetMethods(_memberFlags))
            {
                if (!m.Name.EndsWith(dotName))
                    continue;

                result = type.GetMethod(m.Name, _memberFlags, _binder, paramTypes, null);
                if (result != null)
                    return true;
            }

            return TryGetInterfaceMethod(type, name, paramTypes, out result);
        }

        bool TryGetInterfaceMethod(Type type, string name, Type[] paramTypes, out MethodInfo result)
        {
            foreach (var i in type.GetInterfaces())
            {
                result = i.GetMethod(name, _memberFlags, _binder, paramTypes, null);
                if (result != null ||
                        TryGetInterfaceMethod(i, name, paramTypes, out result))
                    return true;
            }

            result = null;
            return false;
        }
    }
}
