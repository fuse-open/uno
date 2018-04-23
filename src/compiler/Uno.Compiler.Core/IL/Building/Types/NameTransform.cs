using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Building.Types
{
    class NameTransform : CompilerPass, IVisitor
    {
        readonly HashSet<string> _reserved = new HashSet<string>();

        public NameTransform(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Begin()
        {
            Traverse(this);
            return true;
        }

        public bool Visit(Namespace ns)
        {
            var name = ns.Name;
            int overloads = 0;

            while (Backend.IsReserved(ns.Name))
                ns.SetName(name + ++overloads);

            ns.Types.Sort();
            _reserved.Clear();
            SetNames(ns.Types);
            return true;
        }

        public bool Visit(DataType dt)
        {
            if (dt.Tag is List<Member>)
                return true;

            if (!dt.IsMasterDefinition)
            {
                Visit(dt.MasterDefinition);
                dt.Tag = dt.MasterDefinition.Tag;

                foreach (var e in dt.EnumerateMembers(true))
                    e.SetName(e.MasterDefinition.Name);

                return true;
            }

            dt.Events.Sort();
            dt.Properties.Sort();
            dt.Methods.Sort();
            dt.Operators.Sort();
            dt.Constructors.Sort();
            dt.Casts.Sort();

            var members = new List<Member>();
            dt.Tag = members;

            if (dt.Base != null)
            {
                Visit(dt.Base);
                members.AddRange((List<Member>)dt.Base.Tag);
            }

            int start = members.Count;

            foreach (var f in dt.Events)
                if (f.OverriddenEvent != null)
                    f.SetName(f.OverriddenEvent.Name);
                else
                    members.Add(f);

            foreach (var f in dt.Properties)
                if (f.OverriddenProperty != null)
                    f.SetName(f.OverriddenProperty.Name);
                else
                    members.Add(f);

            foreach (var f in dt.EnumerateFunctions())
            {
                if (f is Finalizer)
                    continue;

                var m = f as Method;
                if (m?.OverriddenMethod != null)
                    m.SetName(m.OverriddenMethod.Name);
                else
                    members.Add(f);
            }

            members.AddRange(dt.EnumerateFields());

            // Create new names
            _reserved.Clear();
            SetNames(dt, members, start);
            SetNames(dt.NestedTypes);

            if (dt.IsGenericDefinition)
                SetNames(dt.GenericParameters);

            return true;
        }

        public void Visit(Function f)
        {
            if (!f.IsMasterDefinition)
                for (int i = 0; i < f.Parameters.Length; i++)
                    f.Parameters[i].SetName(f.MasterDefinition.Parameters[i].Name);
            else
            {
                _reserved.Clear();

                Reserve(f.DeclaringType.Name);
                if (f.DeclaringType.Tag is List<Member>)
                    foreach (var e in (List<Member>)f.DeclaringType.Tag)
                        Reserve(e.Name);

                // Rename parameter names in UXL overridden methods to avoid collisions. Parameters are referred to using $0..$N in UXL.
                if (Environment.HasImplementation(f))
                    foreach (var p in f.Parameters)
                        p.SetName(p.Name + "_");

                foreach (var p in f.Parameters)
                    p.SetName(Get(p.Name));
            }
        }

        public override bool Begin(Function f)
        {
            _reserved.Clear();

            Reserve(f.DeclaringType.Name);
            if (f.DeclaringType.Tag is List<Member>)
                foreach (var e in (List<Member>)f.DeclaringType.Tag)
                    Reserve(e.Name);
            foreach (var p in f.Parameters)
                Reserve(p.Name);

            return true;
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.FixedArrayDeclaration:
                {
                    var p = ((FixedArrayDeclaration) e).Variable;
                    p.SetName(Get(p.Name));
                    break;
                }
                case StatementType.VariableDeclaration:
                {
                    for (var p = ((VariableDeclaration) e).Variable; p != null; p = p.Next)
                        p.SetName(Get(p.Name));
                    break;
                }
                case StatementType.TryCatchFinally:
                {
                    var c = ((TryCatchFinally) e).CatchBlocks;
                    foreach (var p in c)
                        p.Exception.SetName(Get(p.Exception.Name));
                    break;
                }
            }
        }

        void SetNames(GenericParameterType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var identifier = type.UnoName.ToIdentifier();
                var overloads = 0;

                for (int j = i - 1; j >= 0; j--)
                    if (types[j].UnoName == type.UnoName)
                        overloads++;

                var name = overloads > 0
                    ? identifier + overloads
                    : identifier;

                while (IsReserved(name))
                    name = identifier + ++overloads;

                Reserve(name);
                type.SetName(name);
            }
        }

        void SetNames(List<DataType> types)
        {
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var identifier = type.UnoName.ToIdentifier();
                var overloads = 0;

                for (int j = i - 1; j >= 0; j--)
                    if (types[j].UnoName == type.UnoName)
                        overloads++;

                var name = overloads > 0
                    ? identifier + overloads
                    : identifier;

                while (IsReserved(name, true))
                    name = identifier + ++overloads;

                Reserve(name, true);
                type.SetName(name);
            }
        }

        void SetNames(DataType parent, List<Member> members, int start)
        {
            for (int i = 0; i < start; i++)
                Reserve(members[i].Name);

            for (int i = start; i < members.Count; i++)
            {
                var member = members[i];
                var identifier = member.UnoName.ToIdentifier();
                var overloads = 0;

                for (int j = i - 1; j >= 0; j--)
                    if (members[j].UnoName == member.UnoName ||
                        members[j].Name == member.UnoName)
                        overloads++;

                var name = overloads > 0
                        ? identifier + overloads :
                    members[i].UnoName == parent.UnoName
                        ? identifier + "0"
                        : identifier;

                while (IsReserved(name))
                    name = identifier + ++overloads;

                Reserve(name);
                member.SetName(name);
            }
        }

        string Get(string name, bool caseInsensitive = false)
        {
            var original = name = name.ToIdentifier();
            var overloads = 0;

            while (IsReserved(name, caseInsensitive))
                name = original + ++overloads;

            Reserve(name, caseInsensitive);
            return name;
        }

        void Reserve(string name, bool caseInsensitive = false)
        {
            _reserved.Add(name);

            if (caseInsensitive)
                _reserved.Add(name.ToUpperInvariant());
        }

        bool IsReserved(string name, bool caseInsensitive = false)
        {
            return _reserved.Contains(name) ||
                caseInsensitive && _reserved.Contains(name.ToUpperInvariant()) ||
                Backend.IsReserved(name);
        }
    }
}
