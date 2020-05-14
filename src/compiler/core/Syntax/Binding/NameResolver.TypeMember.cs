using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver
    {
        static object TryGetPropertyLikeMember(DataType dt, string name)
        {
            foreach (var l in dt.Literals)
                if (l.UnoName == name)
                    return l;

            foreach (var field in dt.Fields)
                if (field.UnoName == name)
                    return field;

            foreach (var ev in dt.Events)
                if (ev.UnoName == name)
                    return ev;

            foreach (var prop in dt.Properties)
                if (prop.UnoName == name && prop.Parameters.Length == 0)
                    return prop;

            return null;
        }

        void FindMethods(DataType dt, string name, int? typeParamCount, ref List<Method> methodGroup, int hideLength)
        {
            foreach (var m in dt.Methods)
            {
                if (m.UnoName == name)
                {
                    if (typeParamCount != null && (!m.IsGenericDefinition || m.GenericParameters.Length != typeParamCount.Value))
                        continue;

                    if (methodGroup == null)
                        methodGroup = new List<Method>();
                    else
                    {
                        bool found = false;

                        for (int i = 0; i < hideLength; i++)
                        {
                            var fm = methodGroup[i];

                            if (fm.OverriddenMethod == m)
                            {
                                found = true;
                                break;
                            }

                            if (m.IsGenericDefinition && fm.IsGenericDefinition && m.GenericParameters.Length == fm.GenericParameters.Length)
                            {
                                for (int j = 0; j < m.GenericParameters.Length; j++)
                                {
                                    var a = m.GenericParameters[j];
                                    var b = fm.GenericParameters[j];

                                    // Avoid parameterizing if it later gives error
                                    if (a.ConstraintType != b.ConstraintType ||
                                        a.Constructors.Count != b.Constructors.Count)
                                        goto CONTINUE;
                                }

                                var pm = _compiler.TypeBuilder.Parameterize(m.Source, m, fm.GenericParameters);

                                if (fm.CompareParameters(pm))
                                {
                                    found = true;
                                    break;
                                }
                            }

                        CONTINUE:
                            if (fm.CompareParameters(m))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            continue;
                    }

                    methodGroup.Add(m);
                }
            }
        }

        object TryGetTypeMemberInternal(DataType dt, string name, int? typeParamCount)
        {
            dt.PopulateMembers();

            if (typeParamCount == null)
            {
                var m = TryGetPropertyLikeMember(dt, name);
                if (m != null)
                    return m;
            }

            // Check if it as a method group
            List<Method> methodGroup = null;
            FindMethods(dt, name, typeParamCount, ref methodGroup, 0);

            // Don't hide inherited methods when a method was not found in current class. This will result in an inconsistent
            // member traversal order that will break in (legal) cases where e.g. a property hides an inherited method.
            if (methodGroup != null)
            {
                if (dt.IsInterface)
                {
                    // Hide methods from "most derived" interface
                    int hideCount = methodGroup.Count;
                    foreach (var it in dt.Interfaces)
                        FindMethods(it, name, typeParamCount, ref methodGroup, hideCount);
                }
                else
                    for (var bt = dt.Base; bt != null; bt = bt.Base)
                        FindMethods(bt, name, typeParamCount, ref methodGroup, methodGroup.Count);

                return methodGroup;
            }

            // Inner type
            var innerType = TryGetMemberCached(dt, name, typeParamCount);
            if (innerType != null)
                return innerType;

            // Check if the base type contains this symbol
            if (dt.Base != null)
            {
                var btMember = TryGetTypeMemberCached(dt.Base, name, typeParamCount);
                if (btMember != null)
                    return btMember;
            }

            // Try resolve in subinterface(s)
            if (dt.IsInterface)
            {
                List<object> subInterfaceMembers = null;

                foreach (var it in dt.Interfaces)
                {
                    var itMember = TryGetTypeMemberCached(it, name, typeParamCount);

                    if (itMember == null)
                        continue;

                    if (subInterfaceMembers == null)
                        subInterfaceMembers = new List<object>();

                    var found = false;

                    foreach (var m in subInterfaceMembers)
                    {
                        if (m == itMember)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        subInterfaceMembers.Add(itMember);
                }

                if (subInterfaceMembers != null)
                {
                    if (subInterfaceMembers.Count == 1)
                        return subInterfaceMembers[0];

                    var mergedMethods = TryMergeMethodArrays(subInterfaceMembers);
                    if (mergedMethods != null)
                        return mergedMethods;

                    return subInterfaceMembers;
                }
            }

            return null;
        }

        static IReadOnlyList<Method> TryMergeMethodArrays(List<object> list)
        {
            List<Method> methods = new List<Method>();

            foreach (var i in list)
            {
                var m = i as IReadOnlyList<Method>;

                if (m != null)
                    methods.AddRange(m);
                else
                    return null;
            }

            for (int i = methods.Count - 1; i > 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (methods[i] == methods[j])
                    {
                        methods.RemoveAt(i);
                        break;
                    }
                }
            }

            return methods;
        }

        internal object TryGetTypeMemberCached(DataType dt, string name, int? typeParamCount)
        {
            var key = new NamescopeKey(dt, name, typeParamCount ?? 0);

            object result;
            if (!_typeMembers.TryGetValue(key, out result))
            {
                result = TryGetTypeMemberInternal(dt, name, typeParamCount);
                if (!dt.Stats.HasFlag(EntityStats.PopulatingMembers) || result != null)
                    _typeMembers[key] = result;
            }

            return result;
        }
    }
}
