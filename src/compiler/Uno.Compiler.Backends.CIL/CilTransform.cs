using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.CIL
{
    class CilTransform : Pass
    {
        readonly Dictionary<Method, Method> _twinMethods = new Dictionary<Method, Method>();

        public CilTransform(CilBackend backend)
            : base(backend)
        {
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            if (e is CallMethod)
            {
                var methodExpr = (CallMethod) e;
                OnTwinMethod(ref methodExpr.Method);
            }
        }

        public override bool Begin(DataType dt)
        {
            foreach (var m in dt.Methods)
            {
                var refMethod = m;
                OnTwinMethod(ref refMethod);
            }

            return true;
        }

        private void OnTwinMethod(ref Method method)
        {
            Method twin;
            if (!_twinMethods.TryGetValue(method, out twin))
            {
                if (method.HasAttribute(Essentials.DotNetOverrideAttribute) &&
                    method.DeclaringType.HasAttribute(Essentials.DotNetTypeAttribute))
                {
                    if (method.IsGenericParameterization)
                    {
                        var def = method.GenericDefinition;
                        OnTwinMethod(ref def);
                        twin = ILFactory.Parameterize(method.Source, def, method.GenericArguments);
                    }
                    else
                    {
                        var twinClass = GetOrCreateTwinClass(method.DeclaringType);

                        if (method.IsGenericDefinition)
                        {
                            var generic = new ClassType(method.Source, twinClass, method.DocComment, Modifiers.Private | Modifiers.Static | Modifiers.Generated, method.UnoName);
                            generic.MakeGenericDefinition(method.GenericParameters);
                            twin = new Method(method.Source, twinClass, method.DocComment, method.Modifiers, method.Name, generic, method.ReturnType, method.Parameters, method.Body);
                            generic.Methods.Add(twin);
                        }
                        else
                            twin = new Method(method.Source, twinClass, method.DocComment, method.Modifiers, method.Name, method.ReturnType, method.Parameters, method.Body);

                        twin.Stats |= method.Stats & EntityStats.ImplicitReturn;
                        twin.SetPrototype(method);
                        twinClass.Methods.Add(twin);
                    }
                }

                _twinMethods.Add(method, twin);
            }

            if (twin != null)
                method = twin;
        }

        private DataType GetOrCreateTwinClass(DataType originalType)
        {
            var twinName = originalType.Name + "_";

            if (originalType.IsNestedType)
            {
                foreach (var type in originalType.ParentType.NestedTypes)
                    if (type.Name == twinName)
                        return type;

                var twinType = new ClassType(originalType.Source, originalType.ParentType,
                    originalType.DocComment, Modifiers.Public | Modifiers.Generated | Modifiers.Static, twinName);
                originalType.ParentType.NestedTypes.Add(twinType);
                return twinType;
            }
            else
            {
                foreach (var type in originalType.ParentNamespace.Types)
                    if (type.Name == twinName)
                        return type;

                var twinType = new ClassType(originalType.Source, originalType.ParentNamespace,
                    originalType.DocComment, Modifiers.Public | Modifiers.Generated | Modifiers.Static, twinName);
                originalType.ParentNamespace.Types.Add(twinType);
                return twinType;
            }
        }
    }
}
