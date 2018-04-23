using System;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Statements
{
    public static class Extensions
    {
        public static void VisitNullableBody(this Pass p, ref Statement s, Statement parent)
        {
            if (s != null)
            {
                p.Next(parent);

                if (s is Scope)
                {
                    var scope = (Scope) s;
                    scope.Visit(p);
                }
                else
                {
                    // Create a virtual scope for the body and visit it
                    var scope = new Scope(s.Source, s);
                    scope.Visit(p);

                    // Unwrap the virtual scope if it's unchanged
                    s = scope.Statements.Count == 1 && scope.Statements[0] == s
                        ? scope.Statements[0]
                        : scope;
                }
            }
        }

        public static void VisitNullable(this Pass p, ref Statement s)
        {
            if (s != null)
            {
                p.Begin(ref s);

                if (s is Expression)
                {
                    var e = s as Expression;
                    p.Begin(ref e, ExpressionUsage.Statement);
                    s = e;
                }

                s.Visit(p);

                if (s is Expression)
                {
                    var e = s as Expression;
                    p.End(ref e, ExpressionUsage.Statement);
                    s = e;
                }

                p.End(ref s);
            }
        }

        public static Statement CopyNullable(this Statement s, CopyState state)
        {
            return s?.CopyStatement(state);
        }

        public static Expression CopyNullable(this Expression e, CopyState state)
        {
            return e?.CopyExpression(state);
        }

        public static Scope CopyNullable(this Scope s, CopyState state)
        {
            return (Scope) s?.CopyStatement(state);
        }
    }
}
