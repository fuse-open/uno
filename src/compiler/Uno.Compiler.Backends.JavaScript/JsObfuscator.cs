using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.API.Utilities;

namespace Uno.Compiler.Backends.JavaScript
{
    class JsObfuscator : Pass
    {
        readonly JsBackend Backend;
        readonly IdentifierGenerator Generator;
        readonly HashSet<DataType> ProcessedTypes = new HashSet<DataType>();
        readonly Dictionary<DataType, int> MemberCounts = new Dictionary<DataType, int>();
        readonly List<IEntity> Globals = new List<IEntity>();

        public JsObfuscator(JsBackend backend)
            : base(backend)
        {
            Backend = backend;
            Generator = new IdentifierGenerator(backend);
        }

        public override bool Begin(Namespace ns)
        {
            if (!ns.IsRoot)
                Backend.Namespaces.Add(ns);

            return true;
        }

        string GetTypeName(DataType dt)
        {
            var result = dt.Name;
            var p = dt.Parent;

            while (!p.IsRoot)
            {
                result = p.Name + "." + result;
                p = p.Parent;
            }

            return result;
        }

        public override bool Begin(DataType dt)
        {
            if (ProcessedTypes.Contains(dt))
                return true;

            ProcessedTypes.Add(dt);

            switch (dt.TypeType)
            {
                case TypeType.Interface:
                    if (Backend.Obfuscate)
                    {
                        foreach (var f in dt.Properties)
                            Globals.Add(f);

                        foreach (var f in dt.Methods)
                            Globals.Add(f);
                    }
                    else
                    {
                        var name = GetTypeName(dt);

                        foreach (var f in dt.Properties)
                            Backend.Globals.Add(f, name + "." + f.Name);

                        foreach (var f in dt.Methods)
                            Backend.Globals.Add(f, name + "." + f.Name);
                    }

                    break;

                case TypeType.Struct:
                case TypeType.Class:
                    // Add implicit property fields (events already added)
                    foreach (var p in dt.Properties)
                        if (p.ImplicitField != null)
                            dt.Fields.Add(p.ImplicitField);

                    if (Backend.Obfuscate)
                    {
                        /*if (Compiler.Extensions.IsNativeType(dt))
                            Backend.GlobalAliases.Add(dt, Compiler.Extensions.TryGetNativeType(dt));
                        else*/ if (dt.IsStatic)
                            Backend.Globals.Add(dt, GetTypeName(dt));
                        else
                            Globals.Add(dt);

                        int memberCount = 0;

                        if (dt.Base != null)
                        {
                            Begin(dt.Base.MasterDefinition);
                            memberCount = MemberCounts[dt.Base.MasterDefinition];
                        }

                        foreach (var f in dt.Fields)
                            if (f.IsStatic)
                                Globals.Add(f);
                            else
                                Backend.Members.Add(f, Generator.Get(memberCount++));

                        foreach (var f in dt.Properties)
                            if (f.IsStatic)
                                Globals.Add(f);
                            else if (f.OverriddenProperty != null)
                                Backend.Members.Add(f, Backend.Members[f.OverriddenProperty.MasterDefinition]);
                            else
                                Backend.Members.Add(f, Generator.Get(memberCount++));

                        foreach (var f in dt.Methods)
                            if (f.IsStatic)
                                Globals.Add(f);
                            else if (f.OverriddenMethod != null)
                                Backend.Members.Add(f, Backend.Members[f.OverriddenMethod.MasterDefinition]);
                            else
                                Backend.Members.Add(f, Generator.Get(memberCount++));

                        foreach (var f in dt.Constructors)
                            Globals.Add(f);

                        MemberCounts.Add(dt, memberCount);
                    }
                    else
                    {
                        var name = GetTypeName(dt);
                        Backend.Globals.Add(dt, name);

                        foreach (var f in dt.Fields)
                            if (f.IsStatic)
                                Backend.Globals.Add(f, name + "." + f.Name);
                            else
                                Backend.Members.Add(f, f.Name);

                        foreach (var f in dt.Properties)
                            if (f.IsStatic)
                                Backend.Globals.Add(f, name + "." + f.Name);
                            else
                                Backend.Members.Add(f, f.Name);

                        foreach (var f in dt.Methods)
                            if (f.IsStatic)
                                Backend.Globals.Add(f, name + "." + f.Name);
                            else
                                Backend.Members.Add(f, f.Name);

                        foreach (var f in dt.Constructors)
                            Backend.Globals.Add(f, name + "." + f.Name);
                    }

                    break;
            }

            return true;
        }

        public override bool Begin(Function f)
        {
            return false;
        }

        public override void End()
        {
            if (Globals.Count > 0)
            {
                //Globals.Sort((a, b) => b.GetRefCount() - a.GetRefCount());

                int globalCount = 0;

                foreach (var o in Globals)
                    Backend.Globals.Add(o, "$" + IdentifierGenerator.GetRaw(globalCount++));
            }

            foreach (var a in Backend.Members.Keys.ToArray())
            {
                if (a is Property)
                {
                    if ((a as Property).GetMethod != null)
                        Backend.Members.Add((a as Property).GetMethod, Backend.Members[a]);

                    if ((a as Property).SetMethod != null)
                        Backend.Members.Add((a as Property).SetMethod, Backend.Members[a]);
                }
            }

            foreach (var a in Backend.Globals.Keys.ToArray())
            {
                if (a is Property)
                {
                    if ((a as Property).GetMethod != null)
                        Backend.Globals.Add((a as Property).GetMethod, Backend.Globals[a]);

                    if ((a as Property).SetMethod != null)
                        Backend.Globals.Add((a as Property).SetMethod, Backend.Globals[a]);
                }
            }
        }
    }
}
