// This file was generated based on lib/UnoCore/Source/Uno/Platform/GraphicsContextBackend.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public abstract class GraphicsContextBackend : global::Uno.Runtime.Implementation.GraphicsContextHandle
    {
        public static GraphicsContextBackend Instance;

        public GraphicsContextBackend()
        {
        }

        public static void SetInstance(GraphicsContextBackend instance)
        {
            GraphicsContextBackend.Instance = instance;
        }

        public abstract global::OpenGL.GLFramebufferHandle GetBackbufferGLHandle();

        public abstract global::Uno.Int2 GetBackbufferSize();

        public abstract global::Uno.Int2 GetBackbufferOffset();

        public abstract global::Uno.Recti GetBackbufferScissor();

        public abstract int GetRealBackbufferHeight();
    }
}
