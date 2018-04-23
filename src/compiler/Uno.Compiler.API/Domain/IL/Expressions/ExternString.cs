using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class ExternString : Expression
    {
        public DataType Type;
        public string String;
        public Namescope[] Usings;

        public ExternString(Source src, DataType type, string str, Namescope[] usings)
            : base(src)
        {
            Type = type;
            String = str;
            Usings = usings;
        }

        public override ExpressionType ExpressionType => ExpressionType.ExternString;

        public override DataType ReturnType => Type;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append(String);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new ExternString(Source, state.GetType(Type), String, Usings);
        }
    }
}