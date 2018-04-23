using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Utilities.Analyzing
{
    class ReturnFinder : Pass
    {
        bool DidNotReturnNewObject;

        ReturnFinder(Pass parent)
            : base(parent)
        {
        }

        public override void Begin(ref Statement s)
        {
            if (s is Return && !((s as Return).Value is NewObject) && !((s as Return).Value is LoadLocal))
                DidNotReturnNewObject = true;

            // TODO: Check method calls recursively
        }

        public static bool IsAlwaysReturningNewObject(Pass parent, Function f)
        {
            // TODO: UXL can be handled using MethodProperties

            if (!f.HasBody)
                return false;

            var p = new ReturnFinder(parent);
            f.Visit(p);
            return !p.DidNotReturnNewObject;
        }
    }
}