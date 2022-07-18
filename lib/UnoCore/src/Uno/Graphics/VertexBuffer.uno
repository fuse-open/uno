using OpenGL;
using Uno.Graphics.OpenGL;

namespace Uno.Graphics
{
    public class VertexBuffer : DeviceBuffer
    {
        public VertexBuffer(BufferUsage usage)
            : base(usage)
        {
            if defined(OPENGL)
                GLInit(GLBufferTarget.ArrayBuffer);
            else
                build_error;
        }

        public VertexBuffer(int sizeInBytes, BufferUsage usage)
            : this(usage)
        {
            Alloc(sizeInBytes);
        }

        public VertexBuffer(byte[] data, BufferUsage usage)
            : this(usage)
        {
            Update(data);
        }

        public VertexBuffer(float[] data, BufferUsage usage)
            : this(usage)
        {
            Update(data);
        }

        public VertexBuffer(float2[] data, BufferUsage usage)
            : this(usage)
        {
            Update(data);
        }

        public VertexBuffer(float3[] data, BufferUsage usage)
            : this(usage)
        {
            Update(data);
        }

        public VertexBuffer(float4[] data, BufferUsage usage)
            : this(usage)
        {
            Update(data);
        }

        public void Update(float[] data)
        {
            Update(data, sizeof(float));
        }

        public void Update(float2[] data)
        {
            Update(data, sizeof(float2));
        }

        public void Update(float3[] data)
        {
            Update(data, sizeof(float3));
        }

        public void Update(float4[] data)
        {
            Update(data, sizeof(float4));
        }
    }
}
