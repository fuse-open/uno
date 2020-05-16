using System.Text;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class ExternOp : Expression
    {
        public NewObject[] Attributes;
        public DataType Type;
        public string String;
        public Expression Object;
        public Expression[] Arguments;
        public Namescope[] Usings;

        public override ExpressionType ExpressionType => ExpressionType.ExternOp;
        public override DataType ReturnType => Type;

        public ExternOp(Source src, NewObject[] attributes, DataType type, string str, Expression obj, Expression[] args, Namescope[] usings)
            : base(src)
        {
            Attributes = attributes;
            Type = type;
            String = str;
            Object = obj;
            Arguments = args;
            Usings = usings;
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            Arguments.Visit(p);
        }

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            if (Object != null)
            {
                Object.Disassemble(sb, ExpressionUsage.Object);
                sb.Append('.');
            }

            Arguments.Disassemble(sb, "extern" + (Type.IsVoid ? "" : "<" + Type + ">") + "(", ") " + String.ToLiteral());
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new ExternOp(Source, Attributes.Copy(state), state.GetType(Type), String, Object.CopyNullable(state), Arguments.Copy(state), Usings);
        }
    }
}