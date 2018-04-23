using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder
    {
        public void ImplementInterfaces(DataType dt)
        {
            dt.InterfaceMethods.Clear();

            foreach (var m in dt.Methods)
                if (m.ImplementedMethod != null)
                    AddImplementation(dt.InterfaceMethods, m.ImplementedMethod, m);

            foreach (var m in dt.Properties)
            {
                if (m.ImplementedProperty != null)
                {
                    if (m.GetMethod?.ImplementedMethod != null)
                        dt.InterfaceMethods.Add(m.GetMethod.ImplementedMethod, m.GetMethod);
                    if (m.SetMethod?.ImplementedMethod != null)
                        dt.InterfaceMethods.Add(m.SetMethod.ImplementedMethod, m.SetMethod);
                }
            }

            foreach (var m in dt.Events)
            {
                if (m.ImplementedEvent != null)
                {
                    if (m.AddMethod?.ImplementedMethod != null)
                        dt.InterfaceMethods.Add(m.AddMethod.ImplementedMethod, m.AddMethod);
                    if (m.RemoveMethod?.ImplementedMethod != null)
                        dt.InterfaceMethods.Add(m.RemoveMethod.ImplementedMethod, m.RemoveMethod);
                }
            }

            foreach (var it in dt.Interfaces)
            {
                foreach (var m in it.Methods)
                {
                    if (dt.InterfaceMethods.ContainsKey(m))
                        continue;

                    var impl = TryImplementMethod(dt, m);

                    if (impl != null)
                        AddImplementation(dt.InterfaceMethods, m, impl);
                }

                foreach (var m in it.Properties)
                {
                    if (m.GetMethod != null && dt.InterfaceMethods.ContainsKey(m.GetMethod) ||
                        m.SetMethod != null && dt.InterfaceMethods.ContainsKey(m.SetMethod))
                        continue;

                    var impl = TryImplementProperty(dt, m);

                    if (impl != null)
                    {
                        if (m.GetMethod != null && impl.GetMethod != null)
                            dt.InterfaceMethods.Add(m.GetMethod, impl.GetMethod);
                        if (m.SetMethod != null && impl.SetMethod != null)
                            dt.InterfaceMethods.Add(m.SetMethod, impl.SetMethod);
                    }
                }

                foreach (var m in it.Events)
                {
                    if (m.AddMethod != null && dt.InterfaceMethods.ContainsKey(m.AddMethod) ||
                        m.RemoveMethod != null && dt.InterfaceMethods.ContainsKey(m.RemoveMethod))
                        continue;

                    var impl = TryImplementEvent(dt, m);

                    if (impl != null)
                    {
                        if (m.AddMethod != null && impl.AddMethod != null)
                            dt.InterfaceMethods.Add(m.AddMethod, impl.AddMethod);
                        if (m.RemoveMethod != null && impl.RemoveMethod != null)
                            dt.InterfaceMethods.Add(m.RemoveMethod, impl.RemoveMethod);
                    }
                }
            }
        }

        void AddImplementation(Dictionary<Method, Method> map, Method decl, Method impl)
        {
            if (decl.IsGenericParameterization)
                decl = decl.GenericDefinition;
            if (impl.IsGenericDefinition)
                impl = Parameterize(impl.Source, impl, decl.GenericParameters);

            map.Add(decl, impl);
        }

        Method TryImplementMethod(DataType dt, Method method)
        {
            for (var bt = dt; bt != null; bt = bt.Base)
            {
                bt.PopulateMembers();

                if (method.IsGenericDefinition)
                {
                    foreach (var m in bt.Methods)
                    {
                        if (m.IsGenericDefinition &&
                            m.IsPublic && !m.IsStatic &&
                            m.UnoName == method.UnoName &&
                            m.Parameters.Length == method.Parameters.Length &&
                            m.GenericParameters.Length == method.GenericParameters.Length)
                        {
                            var p = Parameterize(method.Source, m, method.GenericParameters);

                            if (p.CompareParameters(method))
                                return p;
                        }
                    }
                }
                else
                {
                    foreach (var m in bt.Methods)
                        if (!m.IsGenericDefinition &&
                            m.IsPublic && !m.IsStatic &&
                            m.UnoName == method.UnoName &&
                            m.CompareParameters(method))
                            return m;
                }
            }

            return null;
        }

        Property TryImplementProperty(DataType dt, Property property)
        {
            for (var bt = dt; bt != null; bt = bt.Base)
            {
                bt.PopulateMembers();

                foreach (var m in bt.Properties)
                    if (m.UnoName == property.UnoName && m.IsPublic && !m.IsStatic && m.CompareParameters(property))
                        return m;
            }

            return null;
        }

        Event TryImplementEvent(DataType dt, Event @event)
        {
            for (var bt = dt; bt != null; bt = bt.Base)
            {
                bt.PopulateMembers();

                foreach (var m in bt.Events)
                    if (m.UnoName == @event.UnoName && m.IsPublic && !m.IsStatic)
                        return m;
            }

            return null;
        }

        Method TryOverrideMethod(Method method)
        {
            if (method.IsVirtualOverride)
            {
                for (var bt = method.DeclaringType.Base; bt != null; bt = bt.Base)
                {
                    bt.PopulateMembers();

                    if (method.IsGenericDefinition)
                    {
                        foreach (var m in bt.Methods)
                        {
                            if (m.IsGenericDefinition &&
                                m.UnoName == method.UnoName &&
                                m.Parameters.Length == method.Parameters.Length &&
                                m.GenericParameters.Length == method.GenericParameters.Length)
                            {
                                var p = Parameterize(method.Source, m, method.GenericParameters);
                                if (p.CompareParameters(method))
                                    return p;
                            }
                        }
                    }
                    else
                    {
                        foreach (var m in bt.Methods)
                            if (!m.IsGenericDefinition &&
                                m.UnoName == method.UnoName &&
                                m.CompareParameters(method))
                                return m;
                    }
                }
            }

            return null;
        }

        Property TryOverrideProperty(Property property)
        {
            if (property.IsVirtualOverride)
            {
                for (var bt = property.DeclaringType.Base; bt != null; bt = bt.Base)
                {
                    bt.PopulateMembers();

                    foreach (var m in bt.Properties)
                        if (m.UnoName == property.UnoName &&
                            m.CompareParameters(property))
                            return m;
                }
            }

            return null;
        }

        Event TryOverrideEvent(Event @event)
        {
            if (@event.IsVirtualOverride)
            {
                for (var bt = @event.DeclaringType.Base; bt != null; bt = bt.Base)
                {
                    bt.PopulateMembers();

                    foreach (var m in bt.Events)
                        if (m.UnoName == @event.UnoName)
                            return m;
                }
            }

            return null;
        }
    }
}
