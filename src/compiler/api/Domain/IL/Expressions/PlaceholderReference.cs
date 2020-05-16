using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class PlaceholderReference : Expression
    {
        public PlaceholderValue Value;

        public PlaceholderReference(PlaceholderValue value)
            : base(value.Source)
        {
            Value = value;
        }

        public override ExpressionType ExpressionType => ExpressionType.PlaceholderReference;

        public override DataType ReturnType => Value.ReturnType;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("meta(" + Value.Property.Name + "[" + Value.Location.NodeIndex + "," + Value.Location.BlockIndex + "])");
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new PlaceholderReference(Value);
        }
    }
}