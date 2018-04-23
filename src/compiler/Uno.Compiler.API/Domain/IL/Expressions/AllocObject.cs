using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class AllocObject : Expression
    {
        public DataType ObjectType;

        public override DataType ReturnType => ObjectType;

        public AllocObject(Source src, DataType ct)
            : base(src)
        {
            ObjectType = ct;
        }

        public override ExpressionType ExpressionType => ExpressionType.AllocObject;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("new default ");
            sb.Append(ObjectType);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new AllocObject(Source, state.GetType(ObjectType));
        }
    }
}