using System;
using System.Collections.Generic;
using System.Linq;
using IKVM.Reflection;
using IKVM.Reflection.Emit;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using ParameterModifier = Uno.Compiler.API.Domain.ParameterModifier;
using Type = IKVM.Reflection.Type;

namespace Uno.Compiler.Backends.CIL
{
    partial class CilGenerator
    {
        void CompileType(CilType data)
        {
            try
            {
                if (data.Definition.Attributes != null)
                    foreach (var a in data.Definition.Attributes)
                        data.Builder.SetCustomAttribute(CreateAttributeBuilder(a));

                foreach (var m in data.Fields)
                    if (m.Definition.Attributes != null)
                        foreach (var a in m.Definition.Attributes)
                            m.Builder.SetCustomAttribute(CreateAttributeBuilder(a));

                foreach (var m in data.Properties)
                    if (m.Definition.Attributes != null)
                        foreach (var a in m.Definition.Attributes)
                            m.Builder.SetCustomAttribute(CreateAttributeBuilder(a));

                foreach (var m in data.Events)
                    if (m.Definition.Attributes != null)
                        foreach (var a in m.Definition.Attributes)
                            m.Builder.SetCustomAttribute(CreateAttributeBuilder(a));

                foreach (var m in data.Constructors)
                {
                    if (m.Definition.Attributes != null)
                        foreach (var a in m.Definition.Attributes)
                            m.Builder.SetCustomAttribute(CreateAttributeBuilder(a));

                    for (int i = 0; i < m.Definition.Parameters.Length; i++)
                    {
                        var p = m.Definition.Parameters[i];
                        data.PopulateParameter(p, m.Builder.DefineParameter(i + 1, p.CilParameterAttributes(), p.Name));
                    }

                    EmitFunction(m.Builder.GetILGenerator(), m.Definition);
                }

                foreach (var m in data.Methods)
                {
                    if (m.Definition.Attributes != null)
                        foreach (var a in m.Definition.Attributes)
                            m.Builder.SetCustomAttribute(CreateAttributeBuilder(a));

                    if (m.Definition.IsPInvokable(_essentials, Log))
                        continue;

                    for (int i = 0; i < m.Definition.Parameters.Length; i++)
                    {
                        var p = m.Definition.Parameters[i];
                        data.PopulateParameter(p, m.Builder.DefineParameter(i + 1, p.CilParameterAttributes(), p.Name));
                    }

                    if (m.Definition.Parameters.Length > 0 && m.Definition.Parameters[0].Modifier == ParameterModifier.This)
                        m.Builder.SetCustomAttribute(new CustomAttributeBuilder(_linker.System_Runtime_CompilerServices_ExtensionAttribute_ctor, new object[0]));

                    var method = m.Definition as Method;
                    if (method?.ImplementedMethod != null)
                    {
                        var im = method.ImplementedMethod;

                        if (im.IsGenericParameterization)
                            im = im.GenericDefinition;

                        data.Builder.DefineMethodOverride(m.Builder, _linker.GetMethod(im));
                    }

                    if (!m.Definition.IsAbstract)
                        EmitFunction(m.Builder.GetILGenerator(), m.Definition);
                }

                var inheritedProperties = new Dictionary<string, PropertyBuilder>();
                var inheritedEvents = new Dictionary<string, EventBuilder>();

                foreach (var mi in FindInheritedInterfaceMethods(data))
                {
                    var name = mi.Name;
                    var prefix = mi.ReflectedType.Namespace + "." + mi.ReflectedType.Name + ".";
                    var rt = ParameterizeReturnType(mi);
                    var pl = ParameterizeParameterList(mi);
                    var mb = data.Builder.DefineMethod(prefix + name, MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Private, rt, pl);
                    var cil = mb.GetILGenerator();
                    cil.BeginScope();
                    cil.Emit(OpCodes.Ldstr, mb.Name);
                    cil.Emit(OpCodes.Newobj, _linker.System_NotSupportedException_ctor);
                    cil.Emit(OpCodes.Throw);
                    cil.EndScope();
                    data.Builder.DefineMethodOverride(mb, mi);

                    var type = 0;
                    if (name.StartsWith("get_", StringComparison.InvariantCulture))
                        type = 1;
                    else if (name.StartsWith("set_", StringComparison.InvariantCulture))
                        type = 2;
                    else if (name.StartsWith("add_", StringComparison.InvariantCulture))
                        type = 3;
                    else if (name.StartsWith("remove_", StringComparison.InvariantCulture))
                        type = 4;

                    if (type != 0)
                    {
                        var pname = prefix + name.Substring(name.IndexOf('_') + 1);

                        switch (type)
                        {
                            case 1:
                            case 2:
                            {
                                PropertyBuilder pb;
                                if (!inheritedProperties.TryGetValue(pname, out pb))
                                {
                                    pb = data.Builder.DefineProperty(pname, PropertyAttributes.None, type == 2 ? pl[0] : rt, Type.EmptyTypes);
                                    inheritedProperties.Add(pname, pb);
                                }

                                switch (type)
                                {
                                    case 1:
                                        pb.SetGetMethod(mb);
                                        break;
                                    case 2:
                                        pb.SetSetMethod(mb);
                                        break;
                                }

                                break;
                            }
                            default:
                            {
                                EventBuilder pb;
                                if (!inheritedEvents.TryGetValue(pname, out pb))
                                {
                                    pb = data.Builder.DefineEvent(pname, EventAttributes.None, pl[0]);
                                    inheritedEvents.Add(pname, pb);
                                }

                                switch (type)
                                {
                                    case 3:
                                        pb.SetAddOnMethod(mb);
                                        break;
                                    case 4:
                                        pb.SetRemoveOnMethod(mb);
                                        break;
                                }

                                break;
                            }
                        }
                    }
                }

                data.Builder.CreateType();
            }
            catch (Exception e)
            {
                Log.Error("Failed to compile .NET type " + data.Definition.Quote() + ": " + e.Message);
            }
        }

        IEnumerable<MethodInfo> FindInheritedInterfaceMethods(CilType data)
        {
            switch (data.Definition.TypeType)
            {
                case TypeType.Struct:
                case TypeType.Class:
                    break;

                default:
                    return new MethodInfo[0];
            }

            var masters = new HashSet<string>();
            var result = new HashSet<MethodInfo>();

            foreach (var it in data.Definition.Interfaces)
            {
                if (!it.HasAttribute(_essentials.DotNetTypeAttribute))
                    continue;

                FindMasterInterfaceMethods(it.MasterDefinition, masters);
                FindInheritedInterfaceMethods(_linker.GetType(it), masters, result);
            }

            return result;
        }

        void FindInheritedInterfaceMethods(Type type, HashSet<string> masters, HashSet<MethodInfo> result)
        {
            if (!type.IsTypeBuilder())
            {
                foreach (var i in type.GetInterfaces())
                    FindInheritedInterfaceMethods(i, masters, result);

                foreach (var mi in type.GetMethods())
                {
                    var master = mi.GetReflectedName();

                    if (mi.DeclaringType == type && !masters.Contains(master))
                    {
                        result.Add(mi);
                        masters.Add(master);
                    }
                }
            }
            else if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();

                foreach (var i in def.GetInterfaces())
                    FindInheritedInterfaceMethods(ParameterizeInterface(type, i), masters, result);

                foreach (var mi in def.GetMethods())
                {
                    var master = mi.GetReflectedName();

                    if (mi.DeclaringType == def && !masters.Contains(master))
                    {
                        result.Add(TypeBuilder.GetMethod(type, mi));
                        masters.Add(master);
                    }
                }
            }
        }

        void FindMasterInterfaceMethods(DataType dt, HashSet<string> result)
        {
            foreach (var it in dt.Interfaces)
                FindMasterInterfaceMethods(it.MasterDefinition, result);

            foreach (var f in dt.EnumerateFunctions())
                if (f.MemberType == MemberType.Method)
                    result.Add(_linker.GetMethod(f).GetReflectedName());
        }

        Type ParameterizeInterface(Type type, Type parameterizable)
        {
            if (parameterizable.IsGenericParameter)
            {
                var def = type.GetGenericTypeDefinition();
                var pargs = def.GetGenericArguments();
                var rargs = type.GetGenericArguments();

                for (int i = 0; i < pargs.Length; i++)
                    if (pargs[i].Name == parameterizable.Name)
                        return rargs[i];
            }
            else if (parameterizable.IsGenericType)
            {
                var pargs = parameterizable.GetGenericArguments();
                var rargs = new Type[pargs.Length];

                for (int i = 0; i < rargs.Length; i++)
                    rargs[i] = ParameterizeInterface(type, pargs[i]);

                return parameterizable.GetGenericTypeDefinition().MakeGenericType(rargs);
            }
            else if (parameterizable.IsArray)
            {
                return ParameterizeInterface(type, parameterizable.GetElementType()).MakeArrayType();
            }

            return parameterizable;
        }

        Type ParameterizeReturnType(MethodInfo mi)
        {
            var type = mi.DeclaringType;

            if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();

                foreach (var md in def.GetMethods())
                    if (md.DeclaringType == def && md.Name == mi.Name) // TODO
                        return ParameterizeInterface(type, md.ReturnType);
            }

            return mi.ReturnType;
        }

        Type[] ParameterizeParameterList(MethodInfo mi)
        {
            var type = mi.DeclaringType;

            if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();

                foreach (var md in def.GetMethods())
                    if (md.DeclaringType == def && md.Name == mi.Name) // TODO
                        return md.GetParameters().Select(x => ParameterizeInterface(type, x.ParameterType)).ToArray();
            }

            return mi.GetParameters().Select(x => x.ParameterType).ToArray();
        }

        CustomAttributeBuilder CreateAttributeBuilder(NewObject s)
        {
            var args = new object[s.Arguments.Length];

            for (int i = 0; i < s.Arguments.Length; i++)
                args[i] = s.Arguments[i].ConstantValue;

            return new CustomAttributeBuilder(_linker.GetConstructor(s.Constructor), args);
        }
    }
}
