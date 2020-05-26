using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Graphics.OpenGL;
using Uno.Runtime.InteropServices;

namespace Uno.Graphics
{
    public abstract class DeviceBuffer : IDisposable
    {
        public int SizeInBytes
        {
            get;
            private set;
        }

        public BufferUsage Usage
        {
            get;
            private set;
        }

        public extern(OPENGL) GLBufferTarget GLBufferTarget
        {
            get;
            private set;
        }

        public extern(OPENGL) GLBufferHandle GLBufferHandle
        {
            get;
            private set;
        }

        protected extern(OPENGL) void GLInit(GLBufferTarget target)
        {
            GLBufferTarget = target;
            GLBufferHandle = GL.CreateBuffer();
        }

        protected void Alloc(int sizeInBytes)
        {
            SizeInBytes = sizeInBytes;

            if defined(OPENGL)
            {
                GL.BindBuffer(GLBufferTarget, GLBufferHandle);
                GL.BufferData(GLBufferTarget, sizeInBytes, IntPtr.Zero, Usage.ToGLBufferUsage());
                GL.BindBuffer(GLBufferTarget, GLBufferHandle.Zero);
            }
        }

        internal DeviceBuffer(BufferUsage usage)
        {
            Usage = usage;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            if defined(OPENGL)
                GL.DeleteBuffer(GLBufferHandle);
            else
                build_error;

            IsDisposed = true;
        }

        public void Update(Array data, int elementSize, int index, int count)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (elementSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(elementSize));
            if (index < 0 || index >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0 || index + count > data.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            CheckDisposed();

            if defined(OPENGL)
            {
                var sizeInBytes = count * elementSize;
                var pin = GCHandle.Alloc(data, GCHandleType.Pinned);
                var addr = pin.AddrOfPinnedObject() + index * elementSize;
                GL.BindBuffer(GLBufferTarget, GLBufferHandle);

                if (sizeInBytes <= SizeInBytes)
                    GL.BufferSubData(GLBufferTarget, 0, sizeInBytes, addr);
                else
                {
                    GL.BufferData(GLBufferTarget, sizeInBytes, addr, Usage.ToGLBufferUsage());
                    SizeInBytes = sizeInBytes;
                }

                GL.BindBuffer(GLBufferTarget, GLBufferHandle.Zero);
                pin.Free();
            }
        }

        public void Update(Array data, int elementSize)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Update(data, elementSize, 0, data.Length);
        }

        public void Update(byte[] data)
        {
            Update(data, sizeof(byte));
        }

        protected void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType() + " was disposed");
        }
    }
}
