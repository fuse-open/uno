using Uno.Compiler.ExportTargetInterop;
using OpenGL;

namespace Uno.Platform
{
    using Xli;

    [extern(DOTNET) DotNetType]
    public abstract class GraphicsContextBackend : Uno.Runtime.Implementation.GraphicsContextHandle
    {
        internal static GraphicsContextBackend Instance;

        static GraphicsContextBackend()
        {
            if defined(CPLUSPLUS)
                Instance = new XliGraphicsContext();
            else if defined(CSHARP)
            @{
                Instance = global::Uno.ApplicationContext.AppHost.GetGraphicsContextBackend();
            @}
        }

        extern(OPENGL)
        public abstract GLFramebufferHandle GetBackbufferGLHandle();
        public abstract int2 GetBackbufferSize();
        public abstract int2 GetBackbufferOffset();
        public abstract Recti GetBackbufferScissor();
        public abstract int GetRealBackbufferHeight();
    }
}
