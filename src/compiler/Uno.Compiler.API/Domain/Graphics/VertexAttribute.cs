using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public class VertexAttribute
    {
        public DataType Type;
        public string Name;
        public Expression AttribType;
        public Expression Buffer;
        public Expression Offset;
        public Expression Stride;

        public VertexAttribute(DataType dt, string name, Expression type, Expression buffer, Expression offset, Expression stride)
        {
            Type = dt;
            Name = name;
            AttribType = type;
            Buffer = buffer;
            Offset = offset;
            Stride = stride;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}