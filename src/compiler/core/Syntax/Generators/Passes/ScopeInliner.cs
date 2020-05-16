using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.Syntax.Generators.Passes
{
    class ScopeInliner : Pass
    {
        readonly HashSet<Scope> InlineScopes;

        ScopeInliner(Pass parent, HashSet<Scope> inlineScopes)
            : base(parent)
        {
            InlineScopes = inlineScopes;
        }

        public override void EndScope(Scope outerScope)
        {
            for (int i = outerScope.Statements.Count - 1; i >= 0; i--)
            {
                var innerScope = outerScope.Statements[i] as Scope;

                if (innerScope != null &&
                    InlineScopes.Contains(innerScope))
                {
                    outerScope.Statements.RemoveAt(i);
                    outerScope.Statements.InsertRange(i, innerScope.Statements);
                }
            }
        }

        public static void Process(Pass parent, Scope scope, HashSet<Scope> inlineScopes)
        {
            scope.Visit(new ScopeInliner(parent, inlineScopes));
        }
    }
}
