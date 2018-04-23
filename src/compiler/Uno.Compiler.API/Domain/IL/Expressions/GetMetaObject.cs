using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class GetMetaObject : Expression
    {
        readonly DataType _dt;
        public override DataType ReturnType => _dt;

        public GetMetaObject(Source src, DataType type)
            : base(src)
        {
            _dt = type;
        }

        public override ExpressionType ExpressionType => ExpressionType.GetMetaObject;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("meta(this)");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new GetMetaObject(Source, state.GetType(_dt));
        }
    }
}