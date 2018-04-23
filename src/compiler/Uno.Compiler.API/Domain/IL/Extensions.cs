using System.Collections.Generic;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL
{
    public static class Extensions
    {
        public static bool IsCompatibleWith(this DataType testClass, DataType targetType)
        {
            return IsSubclassOfOrEqual(testClass, targetType) || (
                    targetType.IsInterface &&
                    IsImplementingInterface(testClass, targetType)
                );
        }

        public static bool IsSubclassOf(this DataType testClass, DataType baseClass)
        {
            return testClass != null && (
                    testClass.Base == baseClass ||
                    IsSubclassOf(testClass.Base, baseClass)
                );
        }

        public static bool IsChildClassOf(this DataType testClass, DataType parentClass)
        {
            return testClass != null && (
                    testClass.Parent == parentClass ||
                    IsChildClassOf(testClass.ParentType, parentClass)
                );
        }

        public static bool IsSubclassOfOrEqual(this DataType testClass, DataType otherClass)
        {
            return testClass != null && (
                    testClass == otherClass ||
                    IsSubclassOf(testClass, otherClass)
                );
        }

        public static bool IsSubclassOfOrEqualConstrained(this DataType testClass, DataType otherClass, DataType pt)
        {
            if (testClass.IsSubclassOfOrEqual(otherClass))
                return true;

            if (testClass.IsGenericParameterization && otherClass.IsGenericParameterization &&
                testClass.MasterDefinition.IsSubclassOfOrEqual(otherClass.MasterDefinition))
            {
                for (int i = 0; i < testClass.GenericArguments.Length; i++)
                {
                    var a = testClass.GenericArguments[i];
                    var b = otherClass.GenericArguments[i];
                    if (a != b && (
                            !b.IsGenericParameter || 
                            a != pt.GenericArguments[b.GenericIndex]))
                        return false;
                }

                return true;
            }

            return false;
        }

        public static bool IsRelatedTo(this DataType testClass, DataType otherClass)
        {
            return testClass != null && otherClass != null && (
                    testClass == otherClass ||
                    testClass.IsSubclassOf(otherClass) ||
                    otherClass.IsSubclassOf(testClass)
                );
        }

        public static bool IsImplementingInterface(this DataType testClass, DataType interfaceType)
        {
            if (testClass == interfaceType)
                return true;

            for (var bt = testClass; bt != null; bt = bt.Base)
                foreach (var i in bt.Interfaces)
                    if (IsImplementingInterface(i, interfaceType))
                        return true;

            return false;
        }

        public static bool ContainsGenericType(this DataType[] list)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].IsGenericType)
                    return true;

            return false;
        }

        public static bool HasTypeParameter(this DataType testClass, DataType paramType)
        {
            if (testClass != null && testClass.IsFlattenedDefinition)
                foreach (var t in testClass.FlattenedParameters)
                    if (t == paramType)
                        return true;

            return false;
        }

        public static IEnumerable<Member> EnumerateMembers(this DataType dt, bool all = false)
        {
            if (dt.Initializer != null)
                yield return dt.Initializer;
            if (dt.Finalizer != null)
                yield return dt.Finalizer;

            foreach (var f in dt.Literals)
                yield return f;
            foreach (var f in dt.Fields)
                yield return f;
            foreach (var f in dt.Methods)
                yield return f;
            foreach (var f in dt.Operators)
                yield return f;
            foreach (var f in dt.Casts)
                yield return f;
            foreach (var f in dt.Constructors)
                yield return f;
            foreach (var f in dt.Properties)
            {
                yield return f;
                if (all)
                {
                    if (f.ImplicitField != null)
                        yield return f.ImplicitField;
                    if (f.GetMethod != null)
                        yield return f.GetMethod;
                    if (f.SetMethod != null)
                        yield return f.SetMethod;
                }
            }
            foreach (var f in dt.Events)
            {
                yield return f;
                if (all)
                {
                    if (f.ImplicitField != null)
                        yield return f.ImplicitField;
                    if (f.AddMethod != null)
                        yield return f.AddMethod;
                    if (f.RemoveMethod != null)
                        yield return f.RemoveMethod;
                }
            }
        }

        public static IEnumerable<Member> EnumerateMembersRecursive(this DataType dt, bool all = false)
        {
            var p = dt;
            do
                foreach (var k in p.EnumerateMembers(all))
                    yield return k;
            while ((p = p.Base) != null);
        }

        public static IEnumerable<Function> EnumerateFunctions(this DataType dt)
        {
            if (dt.Initializer != null)
                yield return dt.Initializer;
            if (dt.Finalizer != null)
                yield return dt.Finalizer;

            foreach (var k in dt.Methods)
                yield return k;
            foreach (var k in dt.Operators)
                yield return k;
            foreach (var k in dt.Casts)
                yield return k;
            foreach (var k in dt.Constructors)
                yield return k;

            foreach (var f in dt.Properties)
            {
                if (f.GetMethod != null)
                    yield return f.GetMethod;
                if (f.SetMethod != null)
                    yield return f.SetMethod;
            }
            foreach (var f in dt.Events)
            {
                if (f.AddMethod != null)
                    yield return f.AddMethod;
                if (f.RemoveMethod != null)
                    yield return f.RemoveMethod;
            }
        }

        public static IEnumerable<Field> EnumerateFields(this DataType dt)
        {
            foreach (var f in dt.Fields)
                yield return f;

            foreach (var f in dt.Properties)
                if (f.ImplicitField != null)
                    yield return f.ImplicitField;
            foreach (var f in dt.Events)
                if (f.ImplicitField != null)
                    yield return f.ImplicitField;
        }

        public static IEnumerable<Namescope> EnumerateNestedScopes(this Namescope root)
        {
            if (root is Namespace)
            {
                var ns = root as Namespace;

                foreach (var c in ns.Namespaces)
                    yield return c;
                foreach (var dt in ns.Types)
                    yield return dt;
                foreach (var b in ns.Blocks)
                    yield return b;
            }

            if (root is DataType)
                foreach (var b in (root as DataType).NestedTypes)
                    yield return b;
            if (root is ClassType)
                foreach (var b in (root as ClassType).Block.NestedBlocks)
                    yield return b;
            if (root is Block)
                foreach (var b in (root as Block).NestedBlocks)
                    yield return b;
        }

        public static IEnumerable<Namespace> EnumerateNamespacesBfs(this Namespace root)
        {
            var q = new Queue<Namespace>();
            q.Enqueue(root);

            while (q.Count > 0)
            {
                root = q.Dequeue();
                yield return root;

                foreach (var ns in root.Namespaces)
                    q.Enqueue(ns);
            }
        }

        public static IEnumerable<DataType> EnumerateDataTypesBfs(this Namespace root)
        {
            var q = new Queue<DataType>();

            foreach (var dt in root.Types)
                q.Enqueue(dt);

            while (q.Count > 0)
            {
                var dt = q.Dequeue();
                yield return dt;

                foreach (var it in dt.NestedTypes)
                    q.Enqueue(it);
            }
        }

        public static Function TryGetDefaultConstructor(this DataType dt)
        {
            for (int i = 0; i < dt.Constructors.Count; i++)
                if (dt.Constructors[i].Parameters.Length == 0)
                    return dt.Constructors[i];

            foreach (var m in dt.Methods)
                if (m.Prototype.IsConstructor && m.Parameters.Length == 0 && !m.ReturnType.IsVoid)
                    return m;

            return null;
        }

        public static Constructor TryGetConstructor(this DataType dt, params DataType[] paramTypes)
        {
            foreach (var m in dt.Constructors)
            {
                if (paramTypes.Length != m.Parameters.Length)
                    goto CONTINUE;

                for (int i = 0; i < paramTypes.Length; i++)
                    if (!m.Parameters[i].Type.Equals(paramTypes[i]))
                        goto CONTINUE;

                return m;
            CONTINUE:
                ;
            }
            return null;
        }

        public static Method TryGetMethod(this DataType dt, string name, bool recursive, params DataType[] paramTypes)
        {
            foreach (var m in dt.Methods)
            {
                if (m.UnoName == name)
                {
                    if (paramTypes.Length != m.Parameters.Length)
                        goto CONTINUE;

                    for (int i = 0; i < paramTypes.Length; i++)
                        if (!m.Parameters[i].Type.Equals(paramTypes[i]))
                            goto CONTINUE;

                    return m;
                }
            CONTINUE:
                ;
            }

            return recursive && dt.Base != null
                ? TryGetMethod(dt.Base, name, recursive, paramTypes)
                : null;
        }

        public static Property TryGetProperty(this DataType dt, string name, bool recursive, params DataType[] paramTypes)
        {
            foreach (var m in dt.Properties)
            {
                if (m.UnoName == name)
                {
                    if (paramTypes.Length != m.Parameters.Length)
                        goto CONTINUE;

                    for (int i = 0; i < paramTypes.Length; i++)
                        if (!m.Parameters[i].Type.Equals(paramTypes[i]))
                            goto CONTINUE;

                    return m;
                }
            CONTINUE:
                ;
            }

            return recursive && dt.Base != null
                ? TryGetProperty(dt.Base, name, true, paramTypes)
                : null;
        }

        public static Field TryGetField(this DataType dt, string name, bool recursive)
        {
            foreach (var p in dt.Fields)
                if (p.UnoName == name)
                    return p;

            return recursive && dt.Base != null
                ? TryGetField(dt.Base, name, true)
                : null;
        }

        public static Event TryGetEvent(this DataType dt, string name, bool recursive)
        {
            foreach (var p in dt.Events)
                if (p.Name == name)
                    return p;

            return recursive && dt.Base != null
                ? TryGetEvent(dt.Base, name, true)
                : null;
        }

        public static Cast TryGetCast(this DataType owner, DataType castFromDt, DataType castToDt = null)
        {
            castToDt = castToDt ?? owner;
            foreach (var p in owner.Casts)
                if (p.ReturnType == castToDt && p.Parameters.Length == 1 && p.Parameters[0].Type == castFromDt)
                    return p;

            return null;
        }

        public static bool IsOverridingMethod(this Method method, Method overriddenMethod)
        {
            while (method.OverriddenMethod != null)
                method = method.OverriddenMethod;

            return method == overriddenMethod;
        }

        public static object TryGetAttribute(this IEntity entity, DataType attribute)
        {
            if (entity != null)
                foreach (var attr in entity.Attributes)
                    if (attr.ReturnType.Equals(attribute) && attr.Arguments.Length == 1)
                        return attr.Arguments[0].ConstantValue;

            return null;
        }

        public static IEnumerable<NewObject> EnumerateAttribute(this IEntity entity, DataType attribute, int argumentCount)
        {
            if (entity != null)
                foreach (var attr in entity.Attributes)
                    if (attr.ReturnType.Equals(attribute) && attr.Arguments.Length == argumentCount)
                        yield return attr;
        }

        public static string TryGetAttributeString(this IEntity entity, DataType attribute)
        {
            return entity.TryGetAttribute(attribute) as string;
        }
        public static NewObject TryGetAttributeObject(this IEntity entity, DataType attribute)
        {
            if (entity != null)
                foreach (var attr in entity.Attributes)
                    if (attr.ReturnType.Equals(attribute))
                        return attr;

            return null;
        }
        public static bool HasAttribute(this IEntity entity, DataType attribute)
        {
            if (entity != null)
                foreach (var attr in entity.Attributes)
                    if (attr.ReturnType.Equals(attribute))
                        return true;

            return false;
        }
        public static bool HasAttribute(this DataType dt, DataType attribute, bool recursive = false)
        {
            if (dt != null)
            {
                dt.AssignAttributes();

                foreach (var attr in dt.Attributes)
                    if (attr.ReturnType.Equals(attribute))
                        return true;

                if (recursive && dt.IsNestedType)
                    return dt.ParentType.HasAttribute(attribute, true);
            }

            return false;
        }

        public static bool HasAttribute(this Member member, DataType attribute, bool recursive = false)
        {
            if (member != null)
            {
                member.AssignAttributes();

                foreach (var attr in member.Attributes)
                    if (attr.ReturnType.Equals(attribute))
                        return true;

                if (recursive)
                    return member.DeclaringType.HasAttribute(attribute, true);
            }

            return false;
        }

        public static bool CompareParameters(this IParametersEntity a, IParametersEntity b)
        {
            return CompareParameters(a.Parameters, b.Parameters);
        }

        public static bool CompareParametersEqualOrSubclassOf(this IParametersEntity a, IParametersEntity b)
        {
            return CompareParametersEqualOrSubclassOf(a.Parameters, b.Parameters);
        }

        static bool CompareParameters(Parameter[] a, Parameter[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
                if (!a[i].Type.Equals(b[i].Type) ||
                    a[i].Modifier == ParameterModifier.Ref && b[i].Modifier != ParameterModifier.Ref ||
                    a[i].Modifier == ParameterModifier.Out && b[i].Modifier != ParameterModifier.Out ||
                    a[i].Modifier == ParameterModifier.Const && b[i].Modifier != ParameterModifier.Const)
                    return false;

            return true;
        }

        static bool CompareParametersEqualOrSubclassOf(Parameter[] a, Parameter[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
                if (!(a[i].Type.Equals(b[i].Type) || a[i].Type.IsReferenceType && a[i].Type.IsSubclassOfOrEqual(b[i].Type)) ||
                    a[i].Modifier == ParameterModifier.Ref && b[i].Modifier != ParameterModifier.Ref ||
                    a[i].Modifier == ParameterModifier.Out && b[i].Modifier != ParameterModifier.Out ||
                    a[i].Modifier == ParameterModifier.Const && b[i].Modifier != ParameterModifier.Const)
                    return false;

            return true;
        }
    }
}
