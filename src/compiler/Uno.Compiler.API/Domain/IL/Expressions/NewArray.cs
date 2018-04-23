using System.Text;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class NewArray : Expression
    {
        public Expression Size;
        public Expression[] Initializers;

        public RefArrayType ArrayType;

        public override DataType ReturnType => ArrayType;

        private NewArray(Source src, RefArrayType arrayType, Expression size, Expression[] initializers)
            : base(src)
        {
            ArrayType = arrayType;
            Size = size;
            Initializers = initializers;
        }

        public NewArray(Source src, RefArrayType arrayType, Expression size)
            : base(src)
        {
            ArrayType = arrayType;
            Size = size;
        }

        public NewArray(Source src, RefArrayType arrayType, Expression[] initializers)
            : base(src)
        {
            ArrayType = arrayType;
            Initializers = initializers;
        }

        public override ExpressionType ExpressionType => ExpressionType.NewArray;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("new " + ArrayType.ElementType + "[");
            Size?.Disassemble(sb);
            sb.Append("]");

            if (Initializers != null)
            {
                sb.Append(" { ");

                for (int i = 0; i < Initializers.Length; i++)
                {
                    sb.CommaWhen(i > 0);
                    Initializers[i].Disassemble(sb);
                }

                sb.Append(" }");
            }
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            if (Size != null)
            {
                p.Begin(ref Size);
                Size.Visit(p);
                p.End(ref Size);
            }

            if (Initializers != null)
            {
                for (int i = 0; i < Initializers.Length; i++)
                {
                    p.Begin(ref Initializers[i]);
                    Initializers[i].Visit(p);
                    p.End(ref Initializers[i]);
                }
            }
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new NewArray(Source, state.GetType(ArrayType), Size.CopyNullable(state), Initializers.Copy(state));
        }
    }
}