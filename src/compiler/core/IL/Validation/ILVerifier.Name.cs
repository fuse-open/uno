using System.Collections.Generic;
using System.Text;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Validation
{
    public partial class ILVerifier
    {
        string GetMangledName(Namescope ns)
        {
            string result;
            if (_mangledNames.TryGetValue(ns, out result))
                return result;

            if (ns is ArrayType)
                result = GetMangledName((ns as ArrayType).ElementType) + "[]";
            else if (ns is GenericParameterType)
            {
                var pt = ns.ParentType;
                result = GetMangledName(pt);

                for (int i = 0; i < pt.FlattenedParameters.Length; i++)
                {
                    if (pt.FlattenedParameters[i] == ns)
                    {
                        result += "`" + i;
                        break;
                    }
                }
            }
            else
            {
                var sb = new StringBuilder();

                if (ns.Parent != null && !ns.Parent.IsRoot)
                {
                    sb.Append(GetMangledName(ns.Parent));
                    sb.Append(".");
                }

                sb.Append(ns.Name);

                if (ns is DataType)
                {
                    var dt = ns as DataType;

                    if (dt.IsGenericParameterization)
                    {
                        sb.Append("<");

                        for (int i = 0; i < dt.GenericArguments.Length; i++)
                        {
                            sb.AppendWhen(i > 0, ",");
                            sb.Append(GetMangledName(dt.GenericArguments[i]));
                        }

                        sb.Append(">");
                    }
                    else
                    {
                        sb.Append("`");

                        if (dt.IsGenericDefinition)
                            sb.Append(dt.GenericParameters.Length);
                        else
                            sb.Append("0");
                    }
                }

                result = sb.ToString();
            }

            _mangledNames.Add(ns, result);
            return result;
        }

        void GetMangledParameterList(Parameter[] list, char begin, char end, StringBuilder sb)
        {
            sb.Append(begin);

            for (int i = 0; i < list.Length; i++)
            {
                sb.AppendWhen(i > 0, ",");
                sb.Append(GetMangledName(list[i].Type));
                sb.AppendWhen(list[i].IsReference, "&");
            }

            sb.Append(end);
        }

        string GetMangledMember(Function f)
        {
            var sb = new StringBuilder(f.Name);

            if (f is Method && (f as Method).IsGenericDefinition)
                sb.Append("`" + (f as Method).GenericParameters.Length);

            GetMangledParameterList(f.Parameters, '(', ')', sb);

            if (f is Cast)
                sb.Append("~" + GetMangledName(f.ReturnType));

            return sb.ToString();
        }

        string GetMangledMember(Property f)
        {
            var sb = new StringBuilder("this");
            GetMangledParameterList(f.Parameters, '[', ']', sb);
            return sb.ToString();
        }

        void OnDuplicate(Source src, string name, Source prev)
        {
            Log.Error(src, ErrorCode.E4121, name.Quote() + " is already defined at " + prev);
        }

        bool VerifyUnique(string name, SourceObject e, Dictionary<string, SourceObject> names)
        {
            SourceObject result;
            if (names.TryGetValue(name, out result))
            {
                OnDuplicate(e.Source, e.ToString(), result.Source);
                return false;
            }

            names.Add(name, e);
            return true;
        }

        bool VerifySimilar(string name, SourceObject e, Namescope p, Dictionary<string, SourceObject> names)
        {
            SourceObject result;
            if (names.TryGetValue(name, out result))
            {
                if (result != e && !(
                        result is DataType && e is DataType ||
                        result is Method && e is Method ||
                        result is Constructor && e is Constructor ||
                        result is Operator && e is Operator ||
                        result is Cast && e is Cast ||
                        result is Property && e is Property
                    ))
                {
                    OnDuplicate(e.Source, (p.IsRoot ? "" : p + ".") + name, result.Source);
                    return false;
                }
            }
            else
                names.Add(name, e);

            return true;
        }

        bool VerifyMemberName(Namescope parent, Member member, Dictionary<string, SourceObject> names)
        {
            return VerifyUnique(member.Name, member, names);
        }

        bool VerifyMemberName(Namescope parent, Function member, Dictionary<string, SourceObject> names)
        {
            return VerifyUnique(GetMangledMember(member), member, names) && VerifySimilar(member.Name, member, parent, names);
        }

        bool VerifyMemberName(Namescope parent, Property member, Dictionary<string, SourceObject> names)
        {
            return member.Parameters.Length == 0 ?
                VerifyUnique(member.Name, member, names) :
                VerifyUnique(GetMangledMember(member), member, names) && VerifySimilar(member.Name, member, parent, names);
        }

        bool VerifyMemberName(Namescope parent, Namescope scope, Dictionary<string, SourceObject> names)
        {
            return VerifyUnique(scope.Name, scope, names);
        }

        bool VerifyMemberName(Namescope parent, DataType dt, Dictionary<string, SourceObject> names)
        {
            return VerifyUnique(dt.Name + "`" + (dt.IsGenericDefinition ? dt.GenericParameters.Length : 0), dt, names) && VerifySimilar(dt.Name, dt, parent, names);
        }

        void VerifyNames(Namespace root)
        {
            var names = new Dictionary<string, SourceObject>();

            foreach (var dt in root.Types)
                VerifyMemberName(root, dt, names);
            foreach (var bl in root.Blocks)
                VerifyMemberName(root, bl, names);
            foreach (var ns in root.Namespaces)
                VerifyMemberName(root, ns, names);

            foreach (var dt in root.Types)
                VerifyNames(dt);
            foreach (var bl in root.Blocks)
                VerifyNames(bl);
            foreach (var ns in root.Namespaces)
                VerifyNames(ns);
        }

        void VerifyNames(Block block)
        {
            var names = new Dictionary<string, SourceObject>();

            foreach (var ib in block.NestedBlocks)
                VerifyMemberName(block, ib, names);

            foreach (var ib in block.NestedBlocks)
                VerifyNames(ib);
        }

        void VerifyNames(DataType dt)
        {
            var names = new Dictionary<string, SourceObject>();

            if (dt.IsGenericDefinition)
                foreach (var tp in dt.GenericParameters)
                    VerifyUnique(tp.Name, tp, names);

            foreach (var it in dt.NestedTypes)
                VerifyMemberName(dt, it, names);

            if ((dt as ClassType)?.Block != null)
                foreach (var ib in (dt as ClassType).Block.NestedBlocks)
                    VerifyMemberName(dt, ib, names);

            foreach (var m in dt.Literals)
                VerifyMemberName(dt, m, names);
            foreach (var m in dt.Fields)
                VerifyMemberName(dt, m, names);
            foreach (var m in dt.Events)
                VerifyMemberName(dt, m, names);
            foreach (var m in dt.Properties)
                VerifyMemberName(dt, m, names);
            foreach (var m in dt.Constructors)
                VerifyMemberName(dt, m, names);
            foreach (var m in dt.Methods)
                VerifyMemberName(dt, m, names);
            foreach (var m in dt.Operators)
                VerifyMemberName(dt, m, names);
            foreach (var m in dt.Casts)
                VerifyMemberName(dt, m, names);

            if (dt.Initializer != null)
                VerifyMemberName(dt, dt.Initializer, names);
            if (dt.Finalizer != null)
                VerifyMemberName(dt, dt.Finalizer, names);

            foreach (var it in dt.NestedTypes)
                VerifyNames(it);

            if ((dt as ClassType)?.Block != null)
                foreach (var ib in (dt as ClassType).Block.NestedBlocks)
                    VerifyNames(ib);
        }

        public override bool Begin()
        {
            VerifyNames(Data.IL);
            return true;
        }
    }
}
