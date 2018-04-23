namespace Uno.Graphics
{
    public struct VertexAttributeInfo
    {
        public VertexAttributeType Type;
        public VertexBuffer Buffer;
        public int BufferStride;
        public int BufferOffset;

        public VertexAttributeInfo(VertexAttributeType type, VertexBuffer buffer, int stride, int offset)
        {
            Type = type;
            Buffer = buffer;
            BufferStride = stride;
            BufferOffset = offset;
        }
    }
}
