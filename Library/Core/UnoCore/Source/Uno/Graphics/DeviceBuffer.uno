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

        protected extern(OPENGL) void GLInit(GLBufferTarget target, int sizeInBytes)
        {
            GLBufferTarget = target;
            GLBufferHandle = GL.CreateBuffer();

            SizeInBytes = sizeInBytes;

            GL.BindBuffer(GLBufferTarget, GLBufferHandle);
            GL.BufferData(GLBufferTarget, sizeInBytes, IntPtr.Zero, GLInterop.ToGLBufferUsage(Usage));
            GL.BindBuffer(GLBufferTarget, GLBufferHandle.Zero);
        }

        protected extern(OPENGL) void GLInit(GLBufferTarget target, byte[] data)
        {
            GLBufferTarget = target;
            GLBufferHandle = GL.CreateBuffer();

            SizeInBytes = data.Length;

            var pin = GCHandle.Alloc(data, GCHandleType.Pinned);
            GL.BindBuffer(GLBufferTarget, GLBufferHandle);
            GL.BufferData(GLBufferTarget, data.Length, pin.AddrOfPinnedObject(), GLInterop.ToGLBufferUsage(Usage));
            GL.BindBuffer(GLBufferTarget, GLBufferHandle.Zero);
            pin.Free();
        }

        [Obsolete("Use the byte[] overload instead")]
        protected extern(OPENGL) void GLInit(GLBufferTarget target, Buffer data)
        {
            GLBufferTarget = target;
            GLBufferHandle = GL.CreateBuffer();

            SizeInBytes = data.SizeInBytes;

            GCHandle pin;
            GL.BindBuffer(GLBufferTarget, GLBufferHandle);
            GL.BufferData(GLBufferTarget, data.SizeInBytes, data.PinPtr(out pin), GLInterop.ToGLBufferUsage(Usage));
            GL.BindBuffer(GLBufferTarget, GLBufferHandle.Zero);
            pin.Free();
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
                throw new ObjectDisposedException("DeviceBuffer");
            else if defined(OPENGL)
                GL.DeleteBuffer(GLBufferHandle);
            else
                build_error;

            IsDisposed = true;
        }

        public void Update(byte[] data)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("DeviceBuffer");
            }
            else if defined(OPENGL)
            {
                var pin = GCHandle.Alloc(data, GCHandleType.Pinned);
                GL.BindBuffer(GLBufferTarget, GLBufferHandle);

                if (data.Length <= SizeInBytes)
                {
                    GL.BufferSubData(GLBufferTarget, 0, data.Length, pin.AddrOfPinnedObject());
                }
                else
                {
                    GL.BufferData(GLBufferTarget, data.Length, pin.AddrOfPinnedObject(), GLInterop.ToGLBufferUsage(Usage));
                    SizeInBytes = data.Length;
                }

                GL.BindBuffer(GLBufferTarget, GLBufferHandle.Zero);
                pin.Free();
            }
            else
            {
                build_error;
            }
        }

        [Obsolete("Use the byte[] overload instead")]
        public void Update(Buffer data)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("DeviceBuffer");
            }
            else if defined(OPENGL)
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
    }
}
