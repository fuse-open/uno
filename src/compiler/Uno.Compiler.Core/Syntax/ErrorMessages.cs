using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax
{
    public static class ErrorMessages
    {
        static int StringDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;
            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),  d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        static IEnumerable<string> FindMatchingSymbols(Compiler c, string id)
        {
            foreach (var t in c.Utilities.FindAllTypes())
            {
                if (t.IsMasterDefinition && t.UnoName == id)
                    yield return t.FullName;

                foreach (var m in t.EnumerateMembers())
                    if (m.UnoName == id)
                        yield return m.ToString();
            }
        }

        static IEnumerable<string> FindSimilarSymbols(IEnumerable<string> c, string id)
        {
            foreach (var s in c)
                if (StringDistance(s, id) < 2)
                    yield return s;
        }

        static IEnumerable<string> FindSimilarSymbols(Compiler c, string id)
        {
            foreach (var t in c.Utilities.FindAllTypes())
            {
                if (StringDistance(t.UnoName, id) < 2)
                    yield return t.FullName;

                foreach (var m in t.EnumerateMembers())
                    if (StringDistance(m.UnoName, id) < 2)
                        yield return m.ToString();
            }
        }

        static IEnumerable<string> FindLocals(FunctionCompiler fc)
        {
            foreach (var vs in fc.VariableScopeStack)
                foreach (var v in vs.Variables.Keys)
                    yield return v;
        }

        public static string GetTypeMemberNotFoundError(this Compiler compiler, AstIdentifier id, DataType dt)
        {
            return dt.FullName.Quote() + " does not contain a member called " + id.Symbol.Quote() + ". " +
                SuggestDidYouMisspell(
                    compiler,
                    id.Symbol,
                    FindSimilarSymbols(
                        dt.EnumerateMembers().Select(x => x.UnoName).Concat(
                        dt.NestedTypes.Select(x => x.FullName)).ToArray(),
                        id.Symbol))
                        + "Could you be missing a package reference?";
        }

        public static string GetNamespaceMemberNotFoundError(this Compiler compiler, AstIdentifier id, Namespace ns)
        {
            return (ns.IsRoot
                    ? "No such type or namespace "
                    : ns.FullName.Quote() + " does not contain type or namespace "
                ) + id.Symbol.Quote() + ". " +
                SuggestDidYouMisspell(
                    compiler,
                    id.Symbol,
                    FindSimilarSymbols(
                        ns.Types.Select(x => x.UnoName).Concat(
                        ns.Namespaces.Select(x => x.FullName)).ToArray(),
                        id.Symbol))
                        + "Could you be missing a package reference?";
        }

        public static string GetUnresolvedIdentifierError(this FunctionCompiler fc, AstIdentifier id, int? typeParamCount)
        {
            return GetUnresolvedIdentifierError(fc.Compiler, id, typeParamCount, FindLocals(fc));
        }

        public static string GetUnresolvedIdentifierError(this Compiler compiler, AstIdentifier id, int? typeParamCount, IEnumerable<string> extraSymbols = null)
        {
            string msg = "There is no identifier named " + id.GetParameterizedSymbol(typeParamCount).Quote() + " accessible in this scope. ";

            if (compiler.Environment.SkipVerboseErrors)
                return msg;

            var syms = FindMatchingSymbols(compiler, id.Symbol).ToArray();

            if (syms.Length > 0)
                return SuggestAlternatives(compiler, msg, syms, true);
            else
                return SuggestGlobalAlternatives(compiler, id, typeParamCount, extraSymbols);
        }

        static string SuggestAlternatives(Compiler compiler, string msg, string[] syms, bool proposeAddingUsing)
        {
            if (syms.Length > 0)
            {
                msg += CandidateList("Did you mean ", syms);

                if (!syms[0].Contains('('))
                {
                    if (proposeAddingUsing && syms[0].Contains('.'))
                    {
                        var ns = syms[0].Substring(0, syms[0].LastIndexOf('.'));
                        if (ns.Contains('`'))
                            return msg;

                        var le = compiler.ILFactory.GetEntity(ns);

                        if (le is Namespace)
                            return msg + "For example, try adding 'using " + ns + ";' to the top of the code file. Could you be missing a package reference?";
                    }
                }

                return msg;
            }

            return msg + "Are you missing a qualifier or using-directive? ";
        }

        static string CandidateList(string msg, string[] sm)
        {
            if (sm.Any())
            {
                var count = Math.Min(sm.Length, 3);
                for (int i = 0; i < count; i++)
                {
                    var e = sm[i].Quote();

                    var k = sm[i];
                    if (k.Contains('(')) k = k.Split('(').First();

                    if (k.Contains('.'))
                        e = k.Split('.').Last().Quote() + " (as in " + sm[i].Quote() + ")";

                    if (sm.Length > 1 && i == count-1)
                        msg += " or " + e;
                    else if (i > 0)
                        msg += ", " + e;
                    else
                        msg += e;
                }

                if (sm.Length > 3)
                    msg += ", or one of " + (sm.Length - 3) + " other similar candidates? ";
                else
                    msg += "? ";

                return msg;
            }

            return "";
        }

        static string SuggestDidYouMisspell(Compiler compiler, string original, IEnumerable<string> similarCandaidates)
        {
            if (compiler.Environment.SkipVerboseErrors)
                return "";

            var arr = similarCandaidates.Where(x => x != original).ToArray();

            if (arr.Length > 0)
                return CandidateList("Did you perhaps misspell ", arr);

            return "";
        }

        static string SuggestGlobalAlternatives(Compiler compiler, AstIdentifier id, int? typeParamCount, IEnumerable<string> extraSymbols)
        {
            var msg = "There is nothing named " + id.GetParameterizedSymbol(typeParamCount).Quote() + " accessible in this scope. ";

            if (id.Symbol.Length < 2)
                return msg;

            var similars = FindSimilarSymbols(compiler, id.Symbol);

            if (extraSymbols != null)
                similars = FindSimilarSymbols(extraSymbols, id.Symbol).Concat(similars);

            if (similars.Any())
            {
                msg += SuggestDidYouMisspell(compiler, id.Symbol, similars);
                return msg + " Could you be missing a package reference?";
            }
            else
                msg += "Are you missing a package reference?";

            return msg;
        }
    }
}
