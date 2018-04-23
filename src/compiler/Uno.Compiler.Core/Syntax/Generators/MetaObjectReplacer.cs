using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.Syntax.Generators
{
    class MetaObjectReplacer : Pass
    {
        readonly Expression _obj;

        public MetaObjectReplacer(Pass parent, Expression obj)
            : base(parent)
        {
            _obj = obj;
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            if (e is GetMetaObject)
                e = _obj;
        }
    }
}