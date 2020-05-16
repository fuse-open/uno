using System.Text;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class NewPixelSampler : Expression
    {
        public Expression Texture;
        public Expression OptionalState;
        public DataType SamplerType;

        public override DataType ReturnType => SamplerType;

        public NewPixelSampler(Source src, DataType dt, Expression texture, Expression optionalState)
            : base(src)
        {
            SamplerType = dt;
            Texture = texture;
            OptionalState = optionalState;
        }

        public override ExpressionType ExpressionType => ExpressionType.NewPixelSampler;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("pixel_sampler(");
            Texture.Disassemble(sb);

            if (OptionalState != null)
            {
                sb.Append(", ");
                OptionalState.Disassemble(sb);
            }

            sb.Append(")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.VisitNullable(ref Texture);
            p.VisitNullable(ref OptionalState);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new NewPixelSampler(Source, state.GetType(SamplerType),
                Texture.CopyNullable(state),
                OptionalState.CopyNullable(state));
        }
    }
}