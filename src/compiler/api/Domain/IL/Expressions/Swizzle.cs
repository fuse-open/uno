using System.Text;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class Swizzle : Expression
    {
        public Expression Object;
        public readonly Constructor Constructor;
        public readonly Field[] Fields;

        public override DataType ReturnType => Constructor.DeclaringType;

        public Swizzle(Source src, Constructor ctor, Expression obj, Field[] fields)
            : base(src)
        {
            Object = obj;
            Constructor = ctor;
            Fields = fields;
        }

        public override ExpressionType ExpressionType => ExpressionType.Swizzle;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            Object.Disassemble(sb, ExpressionUsage.Object);
            sb.Append(".");

            for (int i = 0; i < Fields.Length; i++)
                sb.Append(Fields[i].Name);
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Object, ExpressionUsage.Object);
            Object.Visit(p, ExpressionUsage.Object);
            p.End(ref Object, ExpressionUsage.Object);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new Swizzle(Source, state.GetMember(Constructor), Object.CopyExpression(state), Fields.Copy(state));
        }
    }
}