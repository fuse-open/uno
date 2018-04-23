using System;
using System.Collections.Generic;
using System.Reflection;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.CSharp
{
    public static class Extensions
    {
        static bool AreEquivalent(DataType unoType, Type netType)
        {
            // TODO: This logic could be a bit better... (but it actually works for now)
            return true;
        }

        static bool AreEquivalent(Parameter[] unoParams, ParameterInfo[] netParams)
        {
            if (unoParams.Length != netParams.Length)
                return false;

            for (int i = 0; i < unoParams.Length; i++)
                if (!AreEquivalent(unoParams[i].Type, netParams[i].ParameterType))
                    return false;

            return true;
        }

        public static Method TryFindDotNetMethod(this DataType unoType, MethodInfo netMethod)
        {
            if (unoType != null)
                foreach (var m in unoType.Methods)
                    if (m.Name == netMethod.Name &&
                        AreEquivalent(m.ReturnType, netMethod.ReturnType) &&
                        AreEquivalent(m.Parameters, netMethod.GetParameters()))
                        return m;

            return null;
        }

        public static bool ContainsDotNetMethod(this DataType unoType, MethodInfo netMethod)
        {
            return TryFindDotNetMethod(unoType, netMethod) != null;
        }

        public static Property TryFindDotNetProperty(this DataType unoType, PropertyInfo netProperty)
        {
            if (unoType != null)
                foreach (var m in unoType.Properties)
                    if (m.Name == netProperty.Name &&
                        AreEquivalent(m.ReturnType, netProperty.PropertyType) &&
                        AreEquivalent(m.Parameters, netProperty.GetIndexParameters()))
                        return m;

            return null;
        }

        public static bool ContainsDotNetProperty(this DataType unoType, PropertyInfo netProperty)
        {
            return TryFindDotNetProperty(unoType, netProperty) != null;
        }

        public static Event TryFindDotNetEvent(this DataType unoType, EventInfo netEvent)
        {
            if (unoType != null)
                foreach (var m in unoType.Events)
                    if (m.Name == netEvent.Name &&
                        AreEquivalent(m.ReturnType, netEvent.EventHandlerType))
                        return m;

            return null;
        }

        public static bool ContainsDotNetEvent(this DataType unoType, EventInfo netEvent)
        {
            return TryFindDotNetEvent(unoType, netEvent) != null;
        }

        public static Type TryGetDotNetType(this DataType unoType, IEssentials essentials)
        {
            var netName = unoType.TryGetAttributeString(essentials.DotNetTypeAttribute);
            return netName != null ? Type.GetType(netName) : null;
        }

        public static InterfaceType TryFindDotNetInterface(this DataType unoType, Type netInterface, IEssentials essentials)
        {
            if (unoType != null)
                foreach (var it in unoType.Interfaces)
                    if (TryGetDotNetType(it, essentials) == netInterface)
                        return it;

            return null;
        }

        static void FindInheritedDotNetInterfaceMembers(InterfaceType unoInterface, Type netInterface, IEssentials essentials, HashSet<object> visited, HashSet<MemberInfo> result)
        {
            if (unoInterface != null)
            {
                if (visited.Contains(unoInterface))
                    return;

                visited.Add(unoInterface);

                foreach (var it in unoInterface.Interfaces)
                    FindInheritedDotNetInterfaceMembers(it, TryGetDotNetType(it, essentials), essentials, visited, result);
            }

            if (netInterface != null)
            {
                var key = netInterface.ToString();

                if (visited.Contains(key))
                    return;

                visited.Add(key);

                foreach (var m in netInterface.GetInterfaces())
                    FindInheritedDotNetInterfaceMembers(TryFindDotNetInterface(unoInterface, m, essentials), m, essentials, visited, result);

                foreach (var m in netInterface.GetMethods())
                    if (!m.Name.StartsWith("op_", StringComparison.InvariantCulture) &&
                        !m.Name.StartsWith("get_", StringComparison.InvariantCulture) &&
                        !m.Name.StartsWith("set_", StringComparison.InvariantCulture) &&
                        !m.Name.StartsWith("add_", StringComparison.InvariantCulture) &&
                        !m.Name.StartsWith("remove_", StringComparison.InvariantCulture) &&
                        !ContainsDotNetMethod(unoInterface, m))
                        result.Add(m);

                foreach (var m in netInterface.GetProperties())
                    if (!ContainsDotNetProperty(unoInterface, m))
                        result.Add(m);

                foreach (var m in netInterface.GetEvents())
                    if (!ContainsDotNetEvent(unoInterface, m))
                        result.Add(m);
            }
        }

        public static IEnumerable<MemberInfo> EnumerateInheritedDotNetInterfaceMembers(this DataType dt, IEssentials essentials)
        {
            var result = new HashSet<MemberInfo>();
            var visited = new HashSet<object>();

            if (!dt.IsInterface)
                foreach (var it in dt.Interfaces)
                    FindInheritedDotNetInterfaceMembers(it, TryGetDotNetType(it, essentials), essentials, visited, result);

            return result;
        }
    }
}

