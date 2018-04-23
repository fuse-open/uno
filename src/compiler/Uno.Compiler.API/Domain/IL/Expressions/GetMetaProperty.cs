using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class GetMetaProperty : Expression
    {
        public readonly string Name;
        public readonly uint Offset;

        readonly DataType _dt;
        public override DataType ReturnType => _dt;

        public GetMetaProperty(Source src, DataType type, string name, uint offset = 0)
            : base(src)
        {
            Name = name;
            Offset = offset;
            _dt = type;
        }

        public override ExpressionType ExpressionType => ExpressionType.GetMetaProperty;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("meta(" + Name + (Offset != 0 ? ", " + Offset : "") + ")");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new GetMetaProperty(Source, state.GetType(_dt), Name, Offset);
        }
    }
}
