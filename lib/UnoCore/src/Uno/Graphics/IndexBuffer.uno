using OpenGL;
using Uno.Graphics.OpenGL;

namespace Uno.Graphics
{
    public class IndexBuffer : DeviceBuffer
    {
        public IndexBuffer(BufferUsage usage)
            : base(usage)
        {
            if defined(OPENGL)
                GLInit(GLBufferTarget.ElementArrayBuffer);
            else
                build_error;
        }

        public IndexBuffer(int sizeInBytes, BufferUsage usage)
            : this(usage)
        {
            Alloc(sizeInBytes);
        }

        public IndexBuffer(byte[] data, BufferUsage usage)
            : this(usage)
        {
            Update(data);
        }

        public IndexBuffer(ushort[] data, BufferUsage usage)
            : this(usage)
        {
            Update(data);
        }

        public void Update(ushort[] data)
        {
            Update(data, sizeof(ushort));
        }
    }
}
