// This file was generated based on lib/UnoCore/Source/Uno/Graphics/RenderTarget.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Graphics
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public class RenderTarget : global::System.IDisposable
    {
        public RenderTarget()
        {
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                throw new global::System.ObjectDisposedException("RenderTarget");
            else
            {
                if (this.OwnsGLDepthBufferHandle && (this.GLDepthBufferHandle != global::OpenGL.GLRenderbufferHandle.Zero))
                    global::OpenGL.GL.DeleteRenderbuffer(this.GLDepthBufferHandle);

                if (this.OwnsGLFramebufferHandle && (this.GLFramebufferHandle != global::OpenGL.GLFramebufferHandle.Zero))
                    global::OpenGL.GL.DeleteFramebuffer(this.GLFramebufferHandle);
            }

            this.IsDisposed = true;
        }

        public global::Uno.Int2 Size
        {
            get;
            set;
        }

        public bool HasDepth
        {
            get;
            set;
        }

        public global::OpenGL.GLFramebufferHandle GLFramebufferHandle
        {
            get;
            set;
        }

        public global::OpenGL.GLRenderbufferHandle GLDepthBufferHandle
        {
            get;
            set;
        }

        public bool OwnsGLFramebufferHandle
        {
            get;
            set;
        }

        public bool OwnsGLDepthBufferHandle
        {
            get;
            set;
        }

        public bool IsDisposed
        {
            get;
            set;
        }
    }
}
