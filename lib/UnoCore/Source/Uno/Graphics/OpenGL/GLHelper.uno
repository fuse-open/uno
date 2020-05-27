using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Graphics;
using Uno.Diagnostics;
using Uno.Runtime.InteropServices;

namespace Uno.Graphics.OpenGL
{
    public static extern(OPENGL) class GLHelper
    {
        public static void CheckError()
        {
            var err = GL.GetError();

            if (err != GLError.NoError)
                Debug.Log("GL error (" + err.ToString() + ")", DebugMessageType.Error);
        }

        public static void CheckFramebufferStatus()
        {
            var status = GL.CheckFramebufferStatus(GLFramebufferTarget.Framebuffer);

            if (status != GLFramebufferStatus.FramebufferComplete)
                Debug.Log("Incomplete GL framebuffer (" + status + ")", DebugMessageType.Error);
        }

        public static void TexImage2DFromBytes(GLTextureTarget target, int w, int h, int mip, Format format, byte[] data)
        {
            var pin = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                TexImage2DFromIntPtr(target, w, h, mip, format, pin.AddrOfPinnedObject());
            }
            finally
            {
                pin.Free();
            }
        }

        public static void TexImage2DFromIntPtr(GLTextureTarget target, int w, int h, int mip, Format format, IntPtr data)
        {
            switch (format)
            {
            case Format.L8:
                GL.TexImage2D(target, mip, GLPixelFormat.Luminance, w, h, 0, GLPixelFormat.Luminance, GLPixelType.UnsignedByte, data);
                break;

            case Format.LA88:
                GL.TexImage2D(target, mip, GLPixelFormat.LuminanceAlpha, w, h, 0, GLPixelFormat.LuminanceAlpha, GLPixelType.UnsignedByte, data);
                break;

            case Format.RGBA8888:
                GL.TexImage2D(target, mip, GLPixelFormat.Rgba, w, h, 0, GLPixelFormat.Rgba, GLPixelType.UnsignedByte, data);
                break;

            case Format.RGBA4444:
                GL.TexImage2D(target, mip, GLPixelFormat.Rgba, w, h, 0, GLPixelFormat.Rgba, GLPixelType.UnsignedShort4444, data);
                break;

            case Format.RGBA5551:
                GL.TexImage2D(target, mip, GLPixelFormat.Rgba, w, h, 0, GLPixelFormat.Rgba, GLPixelType.UnsignedShort5551, data);
                break;

            case Format.RGB565:
                GL.TexImage2D(target, mip, GLPixelFormat.Rgb, w, h, 0, GLPixelFormat.Rgb, GLPixelType.UnsignedShort565, data);
                break;

            default:
                throw new GLException("Unsupported texture format");
            }
        }

        public static RenderTarget CreateRenderTarget(GLTextureTarget colorTarget, GLTextureHandle colorBuffer, int mip, int2 size, bool depth)
        {
            return CreateRenderTarget(colorTarget, colorBuffer, mip, size, depth ? CreateDepthBuffer(size) : GLRenderbufferHandle.Zero, true);
        }

        public static RenderTarget CreateRenderTarget(GLTextureTarget colorTarget, GLTextureHandle colorBuffer, int mip, int2 size, GLRenderbufferHandle depthBuffer, bool ownsDepthBuffer)
        {
            var result = new RenderTarget();

            var prevHandle = GL.GetFramebufferBinding();

            var handle = GL.CreateFramebuffer();
            result.GLFramebufferHandle = handle;
            result.OwnsGLFramebufferHandle = true;
            result.Size = size;

            GL.BindFramebuffer(GLFramebufferTarget.Framebuffer, handle);
            GL.FramebufferTexture2D(GLFramebufferTarget.Framebuffer, GLFramebufferAttachment.ColorAttachment0, colorTarget, colorBuffer, mip);

            if (depthBuffer != GLRenderbufferHandle.Zero)
            {
                GL.FramebufferRenderbuffer(GLFramebufferTarget.Framebuffer, GLFramebufferAttachment.DepthAttachment, GLRenderbufferTarget.Renderbuffer, depthBuffer);
                result.GLDepthBufferHandle = depthBuffer;
                result.OwnsGLDepthBufferHandle = ownsDepthBuffer;
                result.HasDepth = true;
            }

            GLHelper.CheckFramebufferStatus();
            GLHelper.CheckError();

            GL.BindFramebuffer(GLFramebufferTarget.Framebuffer, prevHandle);

            return result;
        }

        public static GLRenderbufferHandle CreateDepthBuffer(int2 size)
        {
            var prevHandle = GL.GetRenderbufferBinding();

            var handle = GL.CreateRenderbuffer();

            GL.BindRenderbuffer(GLRenderbufferTarget.Renderbuffer, handle);
            GL.RenderbufferStorage(GLRenderbufferTarget.Renderbuffer, GLRenderbufferStorage.DepthComponent16, size.X, size.Y);

            GL.BindRenderbuffer(GLRenderbufferTarget.Renderbuffer, prevHandle);

            return handle;
        }

        public static GLShaderHandle CompileShader(GLShaderType type, string source)
        {
            var handle = GL.CreateShader(type);
            CheckError();

            GL.ShaderSource(handle, source);
            GL.CompileShader(handle);

            if (GL.GetShaderParameter(handle, GLShaderParameter.CompileStatus) != 1)
            {
                var log = GL.GetShaderInfoLog(handle);
                throw new GLException("Error compiling shader (" + type + "):\n\n" + log + "\n\nSource:\n\n" + source);
            }

            CheckError();
            return handle;
        }

        public static GLProgramHandle LinkProgram(GLShaderHandle vertexShader, GLShaderHandle fragmentShader)
        {
            var handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);
            GL.LinkProgram(handle);

            if (GL.GetProgramParameter(handle, GLProgramParameter.LinkStatus) != 1)
            {
                var log = GL.GetProgramInfoLog(handle);
                throw new GLException("Error linking shader program:\n\n" + log);
            }

            GL.UseProgram(handle);

            CheckError();
            return handle;
        }
    }
}
