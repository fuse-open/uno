using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Graphics.OpenGL;

namespace Uno.Graphics
{
    public class RenderTarget : IDisposable
    {
        public int2 Size
        {
            get;
            internal set;
        }

        public bool HasDepth
        {
            get;
            internal set;
        }

        public extern(OPENGL) GLFramebufferHandle GLFramebufferHandle
        {
            get;
            internal set;
        }

        public extern(OPENGL) GLRenderbufferHandle GLDepthBufferHandle
        {
            get;
            internal set;
        }

        internal extern(OPENGL) bool OwnsGLFramebufferHandle
        {
            get;
            set;
        }

        internal extern(OPENGL) bool OwnsGLDepthBufferHandle
        {
            get;
            set;
        }

        internal RenderTarget()
        {
        }

        [DotNetOverride]
        public static RenderTarget Create(texture2D texture, int mip, bool depth)
        {
            if defined(OPENGL)
                return GLHelper.CreateRenderTarget(GLTextureTarget.Texture2D, texture.GLTextureHandle, mip, TextureHelpers.GetMipSize(texture, mip), depth);
            else
                build_error;
        }

        [DotNetOverride]
        public static RenderTarget Create(textureCube texture, CubeFace face, int mip, bool depth)
        {
            if defined(OPENGL)
                return GLHelper.CreateRenderTarget((GLTextureTarget)((int)GLTextureTarget.TextureCubeMapPositiveX + (int)face), texture.GLTextureHandle, mip, TextureHelpers.GetMipSize(texture, mip), depth);
            else
                build_error;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("RenderTarget");
            }
            else if defined(OPENGL)
            {
                if (OwnsGLDepthBufferHandle && GLDepthBufferHandle != GLRenderbufferHandle.Zero)
                    GL.DeleteRenderbuffer(GLDepthBufferHandle);

                if (OwnsGLFramebufferHandle && GLFramebufferHandle != GLFramebufferHandle.Zero)
                    GL.DeleteFramebuffer(GLFramebufferHandle);
            }
            else
            {
                build_error;
            }

            IsDisposed = true;
        }
    }
}
