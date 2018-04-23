using System.Text;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.API.Domain.IL.Expressions
{
    public sealed class LoadVertexAttrib : Expression
    {
        public readonly DrawState State;
        public readonly int Index;

        public override DataType ReturnType => State.VertexAttributes[Index].Type;

        public LoadVertexAttrib(Source src, DrawState state, int index)
            : base(src)
        {
            State = state;
            Index = index;
        }

        public override ExpressionType ExpressionType => ExpressionType.LoadVertexAttrib;

        public override void Disassemble(StringBuilder sb, ExpressionUsage u)
        {
            sb.Append("vertex_attrib::" + State.VertexAttributes[Index].Name);
        }

        public override Expression CopyExpression(CopyState copy)
        {
            return new LoadVertexAttrib(Source, State, Index);
        }
    }
}