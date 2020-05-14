using System.Text;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class LoadElement: Expression
    {
        public Expression Array;
        public Expression Index;

        public LoadElement(Source src, Expression array, Expression index):
            base(src)
        {
            Array = array;
            Index = index;
        }

        public override DataType ReturnType => Array.ReturnType as InvalidType ?? Array.ReturnType.ElementType;

        public override ExpressionType ExpressionType => ExpressionType.LoadElement;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            Array.Disassemble(sb, ExpressionUsage.Object);
            sb.Append("[");
            Index.Disassemble(sb);
            sb.Append("]");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.Begin(ref Array, ExpressionUsage.Object);
            Array.Visit(p, ExpressionUsage.Object);
            p.End(ref Array, ExpressionUsage.Object);

            p.Begin(ref Index);
            Index.Visit(p);
            p.End(ref Index);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new LoadElement(Source, Array.CopyExpression(state), Index.CopyExpression(state));
        }
    }
}