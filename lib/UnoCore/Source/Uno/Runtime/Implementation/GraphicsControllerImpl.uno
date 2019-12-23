using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Graphics;
using Uno.Platform;

namespace Uno.Runtime.Implementation
{
    [extern(DOTNET) DotNetType]
    public abstract class GraphicsContextHandle
    {
    }

    [extern(DOTNET) DotNetType]
    static extern(!MOBILE) class GraphicsControllerImpl
    {
        public static GraphicsContextHandle GetInstance()
        {
            return GraphicsContextBackend.Instance;
        }

        extern(OPENGL)
        public static GLFramebufferHandle GetBackbufferGLHandle(GraphicsContextHandle handle)
        {
            return ((GraphicsContextBackend)handle).GetBackbufferGLHandle();
        }

        public static int2 GetBackbufferSize(GraphicsContextHandle handle)
        {
            return ((GraphicsContextBackend)handle).GetBackbufferSize();
        }

        public static int2 GetBackbufferOffset(GraphicsContextHandle handle)
        {
            return ((GraphicsContextBackend)handle).GetBackbufferOffset();
        }

        public static Recti GetBackbufferScissor(GraphicsContextHandle handle)
        {
            return ((GraphicsContextBackend)handle).GetBackbufferScissor();
        }

        public static int GetRealBackbufferHeight(GraphicsContextHandle handle)
        {
            return ((GraphicsContextBackend)handle).GetRealBackbufferHeight();
        }
    }
}
