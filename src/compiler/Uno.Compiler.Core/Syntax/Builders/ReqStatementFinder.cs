using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.Core.IL;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    class ReqStatementFinder : CompilerPass
    {
        readonly FunctionCompiler _fc;

        public ReqStatementFinder(FunctionCompiler fc)
            : base(fc.Compiler.Pass)
        {
            _fc = fc;
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.GetMetaProperty:
                {
                    var s = e as GetMetaProperty;
                    _fc.AddReqStatement(new ReqProperty(s.Source, s.Name, s.ReturnType, s.Offset, null));
                    break;
                }
                case ExpressionType.GetMetaObject:
                {
                    var s = e as GetMetaObject;
                    _fc.AddReqStatement(new ReqObject(s.Source, s.ReturnType));
                    break;
                }
            }
        }
    }
}