using System.Text;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class NewVertexAttrib : Expression
    {
        public Expression VertexAttributeType;
        public Expression VertexBuffer;
        public Expression VertexBufferStride;
        public Expression VertexBufferOffset;
        public Expression OptionalIndexType;
        public Expression OptionalIndexBuffer;

        readonly DataType _dt;
        public override DataType ReturnType => _dt;

        public NewVertexAttrib(Source src, DataType dt, Expression vertexType, Expression vertexBuffer, Expression vertexStride, Expression vertexOffset, Expression indexType = null, Expression indexBuffer = null)
            : base(src)
        {
            _dt = dt;
            VertexAttributeType = vertexType;
            VertexBuffer = vertexBuffer;
            VertexBufferStride = vertexStride;
            VertexBufferOffset = vertexOffset;
            OptionalIndexType = indexType;
            OptionalIndexBuffer = indexBuffer;
        }

        public override ExpressionType ExpressionType => ExpressionType.NewVertexAttrib;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("vertex_attrib<" + ReturnType + ">(");
            VertexAttributeType.Disassemble(sb);

            sb.Append(", ");
            VertexBuffer.Disassemble(sb);

            sb.Append(", ");
            VertexBufferStride.Disassemble(sb);

            sb.Append(", ");
            VertexBufferOffset.Disassemble(sb);

            if (OptionalIndexType != null)
            {
                sb.Append(", ");
                OptionalIndexType.Disassemble(sb);
            }

            if (OptionalIndexBuffer != null)
            {
                sb.Append(", ");
                OptionalIndexBuffer.Disassemble(sb);
            }

            sb.Append(")");
        }

        public override void Visit(Pass p, ExpressionUsage u)
        {
            p.VisitNullable(ref VertexAttributeType);
            p.VisitNullable(ref VertexBuffer);
            p.VisitNullable(ref VertexBufferOffset);
            p.VisitNullable(ref VertexBufferStride);
            p.VisitNullable(ref OptionalIndexType);
            p.VisitNullable(ref OptionalIndexBuffer);
        }

        public override Expression CopyExpression(CopyState state)
        {
            return new NewVertexAttrib(Source, state.GetType(_dt),
                VertexAttributeType.CopyNullable(state), VertexBuffer.CopyNullable(state),
                VertexBufferStride.CopyNullable(state), VertexBufferOffset.CopyNullable(state),
                OptionalIndexType.CopyNullable(state), OptionalIndexBuffer.CopyNullable(state));
        }
    }
}
