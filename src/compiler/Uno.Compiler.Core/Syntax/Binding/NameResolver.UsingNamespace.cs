using System.Collections.Generic;
using System.Text;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver
    {
        public PartialExpression TryResolveUsingNamespace(Namescope namescope, AstIdentifier id, int? typeParamCount)
        {
            var usings = TryGetUsings(namescope, id.Source);

            if (usings == null || usings.Namespaces.Count == 0)
                return null;

            var pil = new List<PartialExpression>();

            foreach (var ns in usings.Namespaces)
            {
                var pi = TryResolveMember(ns, id, typeParamCount, null);
                if (pi == null) continue;

                if (pi.ExpressionType == PartialExpressionType.Type ||
                    pi.ExpressionType == PartialExpressionType.Block)
                    pil.Add(pi);
            }

            if (pil.Count == 1)
                return pil[0];

            if (pil.Count > 1)
            {
                // Check if it is an overloadable generic type
                if (pil[0].ExpressionType == PartialExpressionType.Type)
                {
                    var parent = (pil[0] as PartialType).Type.Parent;
                    bool allEqual = true;

                    for (int i = 1; i < pil.Count; i++)
                    {
                        if (!(pil[i] is PartialType) || (pil[i] as PartialType).Type.Parent != parent)
                        {
                            allEqual = false;
                            break;
                        }
                    }

                    if (allEqual)
                        return pil[0];
                }

                Log.Error(id.Source, ErrorCode.E3109, id.GetParameterizedSymbol(typeParamCount).Quote() + " is ambiguous" + SuggestCandidates(pil));
                return PartialExpression.Invalid;
            }

            return null;
        }

        internal string SuggestCandidates(IReadOnlyList<object> candidates)
        {
            if (candidates.Count == 0 || _compiler.Environment.SkipVerboseErrors)
                return "";

            const string candidatesAre = "Candidates are: ";
            var sb = new StringBuilder("\n" + candidatesAre + candidates[0] + "\n");

            for (int i = 1; i < candidates.Count; i++)
            {
                sb.Append(new string(' ', candidatesAre.Length) + candidates[i]);
                sb.AppendWhen(i + 1 < candidates.Count, "\n");
            }

            return sb.ToString();
        }

        public NamespaceUsings TryGetUsings(Namescope namescope, Source src)
        {
            var list = GetUsingsList(namescope, src);
            var result = list.Count > 0 ? list[0] : null;

            for (int i = 1; i < list.Count; i++)
                if (src.Line >= list[i].LineNumber)
                    result = list[i];

            return result;
        }

        List<NamespaceUsings> GetUsingsList(Namescope namescope, Source src)
        {
            var key = new NamescopeKey(namescope, src.FullPath, src.Part);

            List<NamespaceUsings> list;
            if (!_usings.TryGetValue(key, out list))
            {
                list = namescope.NamescopeType == NamescopeType.Namespace
                    ? new List<NamespaceUsings>()
                    : GetUsingsList(namescope.Parent, src);

                _usings.Add(key, list);
            }

            return list;
        }

        public Namescope[] GetUsings(Namescope scope, Source src)
        {
            var scopes = new List<Namescope>();
            var p = scope.MasterDefinition;

            while (p != null)
            {
                scopes.Add(p);
                p = p.Parent;
            }

            var usings = TryGetUsings(scope, src);

            if (usings != null)
            {
                foreach (var u in usings.Types)
                    scopes.Add(u);
                foreach (var u in usings.Namespaces)
                    scopes.Add(u);
            }

            return scopes.ToArray();
        }
    }
}
