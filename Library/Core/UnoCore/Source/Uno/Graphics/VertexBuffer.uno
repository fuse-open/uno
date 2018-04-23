using OpenGL;
using Uno.Runtime.Implementation.ShaderBackends.OpenGL;

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
            : base(usage)
        {
            if defined(OPENGL)
                GLInit(GLBufferTarget.ArrayBuffer, sizeInBytes);
            else
                build_error;
        }

        public VertexBuffer(byte[] data, BufferUsage usage)
            : base(usage)
        {
            if defined(OPENGL)
                GLInit(GLBufferTarget.ArrayBuffer, data);
            else
                build_error;
        }

        [Obsolete("Use the byte[] overload instead")]
        public VertexBuffer(Buffer data, BufferUsage usage)
            : base(usage)
        {
            if defined(OPENGL)
                GLInit(GLBufferTarget.ArrayBuffer, data);
            else
                build_error;
        }
    }
}
