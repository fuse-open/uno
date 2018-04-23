using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class TypeOf : Expression, ITypeReference
    {
        public DataType TypeType;
        public DataType Type;

        public override DataType ReturnType => TypeType;

        public TypeOf(Source src, DataType dt, DataType type)
            : base(src)
        {
            TypeType = dt;
            Type = type;
        }

        public override ExpressionType ExpressionType => ExpressionType.TypeOf;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("typeof(" + Type + ")");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new TypeOf(Source, state.GetType(TypeType), state.GetType(Type));
        }

        public DataType ReferencedType => Type;
    }
}