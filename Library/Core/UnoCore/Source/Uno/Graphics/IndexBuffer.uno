using OpenGL;
using Uno.Runtime.Implementation.ShaderBackends.OpenGL;

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
            : base(usage)
        {
            if defined(OPENGL)
                GLInit(GLBufferTarget.ElementArrayBuffer, sizeInBytes);
            else
                build_error;
        }

        public IndexBuffer(byte[] data, BufferUsage usage)
            : base(usage)
        {
            if defined(OPENGL)
                GLInit(GLBufferTarget.ElementArrayBuffer, data);
            else
                build_error;
        }

        [Obsolete("Use the byte[] overload instead")]
        public IndexBuffer(Buffer data, BufferUsage usage)
            : base(usage)
        {
            if defined(OPENGL)
                GLInit(GLBufferTarget.ElementArrayBuffer, data);
            else
                build_error;
        }
    }
}
