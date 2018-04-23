using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Utilities.Analyzing
{
    class ParameterFinder : Pass
    {
        readonly HashSet<int> UnsafeParameters = new HashSet<int>();

        ParameterFinder(Pass parent)
            : base(parent)
        {
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            if (e is LoadArgument && u.IsObject())
                UnsafeParameters.Add((e as LoadArgument).Index);
            else if (e is StoreArgument)
                UnsafeParameters.Add((e as StoreArgument).Index);
        }

        public static HashSet<int> FindUnsafeParameters(Pass parent, Function f)
        {
            var p = new ParameterFinder(parent);
            f.Visit(p);
            return p.UnsafeParameters;
        }
    }
}