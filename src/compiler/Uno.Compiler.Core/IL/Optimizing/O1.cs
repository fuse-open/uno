using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Optimizing
{
    public class O1 : CompilerPass
    {
        public O1(CompilerPass parent)
            : base(parent)
        {
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.AddListener:
                case ExpressionType.RemoveListener:
                case ExpressionType.GetProperty:
                case ExpressionType.SetProperty:
                case ExpressionType.CallMethod:
                case ExpressionType.CallDelegate:
                {
                    var s = (CallExpression) e;
                    if (TryInlineFunction(ref e, s.Function))
                        Begin(ref e, u);
                    break;
                }
            }
        }

        bool TryInlineFunction(ref Expression e, Function f)
        {
            return false;
        }

        class ArgumentSwapper : CompilerPass
        {
            readonly Expression Object;
            readonly Expression[] Arguments;

            public ArgumentSwapper(CompilerPass parent, Expression obj, Expression[] args)
                : base(parent)
            {
                Object = obj;
                Arguments = args;
            }

            public override void Begin(ref Expression e, ExpressionUsage u)
            {
                switch (e.ExpressionType)
                {
                    case ExpressionType.This:
                    {
                        break;
                    }
                    case ExpressionType.LoadArgument:
                    {
                        break;
                    }
                }
            }
        }
    }
}
