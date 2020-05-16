using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Core.Syntax.Binding;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public partial class FunctionCompiler
    {
        static void FindIndexers(DataType dt, List<Property> candidates, int hideCount)
        {
            foreach (var m in dt.Properties)
            {
                if (m.Parameters.Length == 0)
                    continue;

                var found = false;
                for (int i = 0; i < hideCount; i++)
                {
                    var fm = candidates[i];
                    if (fm.OverriddenProperty == m || fm.CompareParameters(m))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    candidates.Add(m);
            }
        }

        public PartialExpression ResolveLookUp(AstCall lu)
        {
            var sym = CompileExpression(lu.Base);
            if (sym.IsInvalid) return PartialExpression.Invalid;

            var candidates = new List<Property>();

            if (sym.ReturnType.IsInterface)
            {
                var dt = sym.ReturnType;
                dt.PopulateMembers();
                FindIndexers(dt, candidates, 0);

                var hideCount = candidates.Count;
                foreach (var it in dt.Interfaces)
                    FindIndexers(it, candidates, hideCount);
            }
            else
            {
                var bt = sym.ReturnType;
                bt.PopulateMembers();

                do
                {
                    FindIndexers(bt, candidates, candidates?.Count ?? 0);
                    bt = bt.Base;
                }
                while (bt != null);
            }

            Property indexer;
            Expression[] args;
            return TryResolveIndexerOverload(lu.Source, candidates, lu.Arguments, out indexer, out args)
                    ? new PartialIndexer(lu.Source, sym.Address, indexer, args) :
                sym.ReturnType.IsArray && args.Length == 1
                    ? new PartialArrayElement(lu.Source, sym, sym.ReturnType.ElementType, CompileImplicitCast(lu.Source, Essentials.Int, args[0]))
                    : PartialError(lu.Source, ErrorCode.E3103, sym.ReturnType.Quote() + " does not contain an indexer that matches the argument list");
        }
    }
}
