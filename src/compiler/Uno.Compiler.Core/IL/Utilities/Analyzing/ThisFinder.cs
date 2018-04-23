using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Utilities.Analyzing
{
    class ThisFinder : Pass
    {
        bool WritesToThis;

        ThisFinder(Pass parent)
            : base(parent)
        {
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            if (e is This && u.IsObject() || e is StoreThis)
                WritesToThis = true;

            // TODO: Check method calls recursively
        }

        public static bool IsWritingToThis(Pass parent, Function f)
        {
            var p = new ThisFinder(parent);
            f.Visit(p);
            return p.WritesToThis;
        }
    }
}