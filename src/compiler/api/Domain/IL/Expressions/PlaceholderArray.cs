using System.Text;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public class PlaceholderArray : Expression
    {
        public readonly DataType FixedArrayType;
        public readonly Expression[] OptionalInitializer;

        public PlaceholderArray(Source src, DataType fat, Expression[] optionalInitializer)
            : base(src)
        {
            FixedArrayType = fat;
            OptionalInitializer = optionalInitializer;
        }

        public override ExpressionType ExpressionType => ExpressionType.PlaceholderArray;

        public override DataType ReturnType => FixedArrayType;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append(FixedArrayType);

            if (OptionalInitializer != null)
            {
                var first = true;
                sb.Append(" { ");

                foreach (var e in OptionalInitializer)
                {
                    sb.CommaWhen(!first);
                    sb.Append(e);
                    first = false;
                }

                sb.AppendWhen(OptionalInitializer.Length > 0, " ");
                sb.Append("}");
            }
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            if (OptionalInitializer != null)
                for (int i = 0; i < OptionalInitializer.Length; i++)
                    p.VisitNullable(ref OptionalInitializer[i]);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new PlaceholderArray(Source, FixedArrayType, OptionalInitializer.Copy(state));
        }
    }
}