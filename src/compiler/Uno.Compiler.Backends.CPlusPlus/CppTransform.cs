using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.CPlusPlus
{
    class CppTransform : Pass
    {
        static readonly object Tag = new object();
        readonly CppBackend _backend;

        public CppTransform(CppBackend backend)
            : base(backend)
        {
            _backend = backend;
        }

        public override bool Begin()
        {
            Traverse(FlattenInterfaces);
            return false;
        }

        void FlattenInterfaces(DataType dt)
        {
            if (dt.Tag == Tag)
                return;

            dt.Tag = Tag;
            var flattenedInterfaceTypes = new List<InterfaceType>();

            // Collect interface implementations from base types
            if (dt.Base != null)
            {
                FlattenInterfaces(dt.Base);
                flattenedInterfaceTypes.AddRange(dt.Base.Interfaces);

                foreach (var e in dt.Base.InterfaceMethods)
                    if (!dt.InterfaceMethods.ContainsKey(e.Key))
                        dt.InterfaceMethods[e.Key] = e.Value;
            }

            foreach (var it in dt.Interfaces)
                if (!flattenedInterfaceTypes.Contains(it))
                    flattenedInterfaceTypes.Add(it);

            dt.SetInterfaces(flattenedInterfaceTypes.ToArray());

            foreach (var f in dt.EnumerateFunctions())
            {
                var m = f as Method;
                var baseMethod = m?.OverriddenMethod;

                if (baseMethod != null)
                {
                    while (baseMethod.OverriddenMethod != null)
                        baseMethod = baseMethod.OverriddenMethod;

                    foreach (var ii in baseMethod.DeclaringType.InterfaceMethods)
                        if (ii.Value == baseMethod)
                            dt.InterfaceMethods[ii.Key] = m;
                }
            }

            // Create "box" wrapper when a struct implementing interface is returned
            foreach (var e in dt.InterfaceMethods.ToArray())
                if (e.Key.ReturnType.IsReferenceType && e.Value.ReturnType.IsValueType)
                    dt.InterfaceMethods[e.Key] = e.Value.DeclaringType == dt
                        ? GetBoxedMethod(dt, e.Key, e.Value)
                        : dt.Base.InterfaceMethods[e.Key];

            // Flag methods implementing interfaces
            foreach (var e in dt.InterfaceMethods)
                e.Value.MasterDefinition.Stats |= EntityStats.ImplementsInterface;
        }

        Method GetBoxedMethod(DataType dt, Method decl, Method impl)
        {
            if (dt.IsMasterDefinition)
            {
                var m = new Method(impl.Source, dt, impl.DocComment, Modifiers.Public | Modifiers.Generated, impl.Name + "_boxed",
                    decl.ReturnType, decl.Parameters, new Scope(impl.Source));

                Variable ret = null;
                if (!impl.ReturnType.IsVoid && _backend.IsConstrained(impl))
                {
                    ret = new Variable(m.Source, m, dt.GetUniqueIdentifier("ret"), impl.ReturnType);
                    m.Body.Statements.Add(new VariableDeclaration(ret));
                }

                var args = new Expression[m.Parameters.Length];
                for (int i = 0; i < args.Length; i++)
                    args[i] = new LoadArgument(m.Source, m, i);

                m.Body.Statements.Add(
                    new Return(m.Source,
                        new CastOp(m.Source, m.ReturnType,
                            new CallMethod(m.Source, new This(m.Source, dt).Address, impl, args, ret))));
                m.SetPrototype(impl);
                dt.Methods.Add(m);
                dt.InterfaceMethods[decl] = m;
                return m;
            }

            foreach (var e in dt.MasterDefinition.InterfaceMethods)
            {
                if (e.Value.Modifiers != (Modifiers.Public | Modifiers.Generated) ||
                    e.Key.MasterDefinition != decl.MasterDefinition)
                    continue;

                var m = new Method(impl.Source, dt,
                    e.Value.DocComment, e.Value.Modifiers, e.Value.Name,
                    decl.ReturnType, decl.Parameters, e.Value.Body);
                m.SetMasterDefinition(e.Value);
                m.SetPrototype(impl);
                dt.Methods.Add(m);
                dt.InterfaceMethods[decl] = m;
                return m;
            }

            Log.Error(impl.Source, ErrorCode.I0000, "C++: Boxed method master definition was not found.");
            return impl;
        }
    }
}
