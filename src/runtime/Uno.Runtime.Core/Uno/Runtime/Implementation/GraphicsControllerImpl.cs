// This file was generated based on lib/UnoCore/Source/Uno/Runtime/Implementation/GraphicsControllerImpl.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Runtime.Implementation
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class GraphicsControllerImpl
    {
        public static GraphicsContextHandle GetInstance()
        {
            return global::Uno.Platform.GraphicsContextBackend.Instance;
        }

        public static global::OpenGL.GLFramebufferHandle GetBackbufferGLHandle(GraphicsContextHandle handle)
        {
            return ((global::Uno.Platform.GraphicsContextBackend)handle).GetBackbufferGLHandle();
        }

        public static global::Uno.Int2 GetBackbufferSize(GraphicsContextHandle handle)
        {
            return ((global::Uno.Platform.GraphicsContextBackend)handle).GetBackbufferSize();
        }

        public static global::Uno.Int2 GetBackbufferOffset(GraphicsContextHandle handle)
        {
            return ((global::Uno.Platform.GraphicsContextBackend)handle).GetBackbufferOffset();
        }

        public static global::Uno.Recti GetBackbufferScissor(GraphicsContextHandle handle)
        {
            return ((global::Uno.Platform.GraphicsContextBackend)handle).GetBackbufferScissor();
        }

        public static int GetRealBackbufferHeight(GraphicsContextHandle handle)
        {
            return ((global::Uno.Platform.GraphicsContextBackend)handle).GetRealBackbufferHeight();
        }
    }
}
