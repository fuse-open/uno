using OpenGL;
using Uno.Graphics.OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Diagnostics;

namespace Uno.Graphics
{
    [TargetSpecificImplementation]
    public sealed intrinsic class VideoTexture : IDisposable
    {
        public readonly bool IsMipmap = false;
        public readonly bool IsPow2 = false;

        public extern(OPENGL) GLTextureHandle GLTextureHandle
        {
            get;
            private set;
        }

        public extern(OPENGL) VideoTexture(GLTextureHandle handle)
        {
            GLTextureHandle = handle;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("VideoTexture");
            else if defined(OPENGL)
                GL.DeleteTexture(GLTextureHandle);
            else
                build_error;

            IsDisposed = true;
        }
    }
}
