using System.Text;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class Constant : Expression
    {
        public object Value;
        public DataType ValueType;

        public override DataType ReturnType => ValueType;
        public override object ConstantValue => Value;
        public override ExpressionType ExpressionType => ExpressionType.Constant;

        public Constant(Source src, DataType valueType, object value)
            : base(src)
        {
            Value = value;
            ValueType = valueType;
        }

        public Constant(DataType valueType, object value)
            : this(Source.Unknown, valueType, value)
        {
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? -1;
        }

        public override bool Equals(object obj)
        {
            var c = obj as Constant;
            return c != null && Equals(Value, c.Value);
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            if (ValueType is EnumType)
            {
                var et = ValueType as EnumType;

                foreach (var l in et.Literals)
                {
                    if (l.Value == Value)
                    {
                        sb.Append(et + "." + l.Name);
                        return;
                    }
                }

                sb.Append("(" + ValueType + ") " + Value.ToLiteral());
            }
            else
                sb.Append(Value.ToLiteral());
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new Constant(Source, state.GetType(ValueType), Value);
        }
    }
}