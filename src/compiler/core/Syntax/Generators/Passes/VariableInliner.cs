using System.Collections.Generic;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.Syntax.Generators.Passes
{
    class VariableInliner
    {
        class LoadLocalCounter : Pass
        {
            public readonly Dictionary<Variable, int> LoadCount = new Dictionary<Variable, int>();

            public LoadLocalCounter(Pass parent)
                : base(parent)
            {
            }

            public override void Begin(ref Statement s)
            {
                switch (s.StatementType)
                {
                    case StatementType.VariableDeclaration:
                        LoadCount[(s as VariableDeclaration).Variable] = 0;
                        break;
                }
            }

            public override void Begin(ref Expression e, ExpressionUsage u)
            {
                switch (e.ExpressionType)
                {
                    case ExpressionType.LoadLocal:
                        {
                            var s = e as LoadLocal;

                            int count;
                            if (LoadCount.TryGetValue(s.Variable, out count))
                                LoadCount[s.Variable] = count + 1;
                        }
                        break;

                    case ExpressionType.StoreLocal:
                        LoadCount.Remove((e as StoreLocal).Variable);
                        break;
                }
            }
        }

        class LoadLocalInliner : Pass
        {
            readonly HashSet<Variable> InlineVariables = new HashSet<Variable>();

            public LoadLocalInliner(Pass parent, Dictionary<Variable, int> loadCount)
                : base(parent)
            {
                foreach (var e in loadCount)
                    if (e.Key.OptionalValue != null && (e.Value <= 1 || Expressions.IsVariableOrGlobal(e.Key.OptionalValue)))
                        InlineVariables.Add(e.Key);
            }

            public override void End(ref Expression e, ExpressionUsage u)
            {
                switch (e.ExpressionType)
                {
                    case ExpressionType.LoadLocal:
                        {
                            var var = (e as LoadLocal).Variable;

                            if (InlineVariables.Contains(var))
                            {
                                e = var.OptionalValue;
                                End(ref e, u);
                            }
                        }
                        break;
                }
            }

            public override void End(ref Statement s)
            {
                switch (s.StatementType)
                {
                    case StatementType.VariableDeclaration:
                        {
                            var vd = s as VariableDeclaration;
                            for (var var = vd.Variable; var != null; var = var.Next)
                                if (InlineVariables.Contains(var))
                                    s = new NoOp(vd.Source);
                        }
                        break;
                }
            }

            public override void EndScope(Scope s)
            {
                for (int i = s.Statements.Count - 1; i >= 0; i--)
                    if (s.Statements[i] is NoOp)
                        s.Statements.RemoveAt(i);
            }
        }

        public static void Process(Pass parent, Scope scope)
        {
            var llc = new LoadLocalCounter(parent);
            scope.Visit(llc);

            var lli = new LoadLocalInliner(parent, llc.LoadCount);
            scope.Visit(lli);
        }

        public static void Process(Pass parent, Shader shader)
        {
            var llc = new LoadLocalCounter(parent);
            shader.Entrypoint.Visit(llc);

            var lli = new LoadLocalInliner(parent, llc.LoadCount);
            shader.Entrypoint.Visit(lli);
        }
    }
}
