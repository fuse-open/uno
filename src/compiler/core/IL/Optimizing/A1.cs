using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Optimizing
{
    class A1 : CompilerPass
    {
        public A1(CompilerPass parent)
            : base(parent)
        {
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.Throw:
                    Function.Stats |= EntityStats.ThrowsException;
                    break;
            }
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.LoadElement:
                case ExpressionType.StoreElement:
                case ExpressionType.CallDelegate:
                case ExpressionType.LoadPtr:
                    Function.Stats |= EntityStats.ThrowsException;
                    break;
                case ExpressionType.CallMethod:
                {
                    var s = (CallMethod) e;
                    if (s.Function.IsVirtual && !(s.Object is Base))
                        Function.Stats |= EntityStats.ThrowsException;
                    break;
                }
                case ExpressionType.CastOp:
                {
                    var s = (CastOp) e;
                    switch (s.CastType)
                    {
                        case CastType.Down:
                        case CastType.Unbox:
                            Function.Stats |= EntityStats.ThrowsException;
                            break;
                    }
                    break;
                }
            }
        }
    }
}