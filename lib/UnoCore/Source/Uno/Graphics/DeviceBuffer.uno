using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Runtime.Implementation.ShaderBackends.OpenGL;
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
                GL.BufferData(GLBufferTarget, sizeInBytes, IntPtr.Zero, GLInterop.ToGLBufferUsage(Usage));
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
                    GL.BufferData(GLBufferTarget, sizeInBytes, addr, GLInterop.ToGLBufferUsage(Usage));
                    SizeInBytes = sizeInBytes;
                }

                GL.BindBuffer(GLBufferTarget, GLBufferHandle.Zero);
                pin.Free();
            }
        }

        public void Update(Array data, int elementSize)
        {
            Update(data, elementSize, 0, data.Length);
        }

        public void Update(byte[] data)
        {
            Update(data, sizeof(byte));
        }

        [Obsolete("Use the byte[] overload instead")]
        public void Update(Buffer data)
        {
            CheckDisposed();

            if defined(OPENGL)
            {
                GL.BindBuffer(GLBufferTarget, GLBufferHandle);

                if (data.SizeInBytes <= SizeInBytes)
                {
                    GL.BufferSubData(GLBufferTarget, 0, data);
                }
                else
                {
                    GL.BufferData(GLBufferTarget, data, GLInterop.ToGLBufferUsage(Usage));
                    SizeInBytes = data.SizeInBytes;
                }

                GL.BindBuffer(GLBufferTarget, GLBufferHandle.Zero);
            }
            else
            {
                build_error;
            }
        }

        protected void CheckDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType() + " was disposed");
        }
    }
}
