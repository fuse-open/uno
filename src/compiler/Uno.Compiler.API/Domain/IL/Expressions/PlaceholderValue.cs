using System.Text;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class PlaceholderValue : Expression
    {
        public readonly MetaProperty Property;
        public readonly MetaLocation Location;
        public readonly MetaStage Stage;
        public Expression Value;

        public PlaceholderValue(MetaProperty mp, MetaLocation loc, Expression value, MetaStage stage)
            : base(value.Source)
        {
            Property = mp;
            Location = loc;
            Stage = stage;
            Value = value;
        }

        public override ExpressionType ExpressionType => ExpressionType.PlaceholderValue;

        public override DataType ReturnType => Property.ReturnType;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            Value.Disassemble(sb, u);
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Value, u);
            Value.Visit(p, u);
            p.End(ref Value, u);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new PlaceholderValue(Property, Location, Value.CopyExpression(state), Stage);
        }
    }
}