// This file was generated based on Library/Core/UnoCore/Source/Uno/Runtime/Implementation/GraphicsControllerImpl.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Runtime.Implementation
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class GraphicsControllerImpl
    {
        public static GraphicsContextHandle GetInstance()
        {
            return global::Uno.ApplicationContext.AppHost.GetGraphicsContext();
        }

        public static global::OpenGL.GLFramebufferHandle GetBackbufferGLHandle(GraphicsContextHandle handle)
        {
            return (handle).GetBackbufferGLHandle();
        }

        public static global::Uno.Int2 GetBackbufferSize(GraphicsContextHandle handle)
        {
            return (handle).GetBackbufferSize();
        }

        public static global::Uno.Int2 GetBackbufferOffset(GraphicsContextHandle handle)
        {
            return (handle).GetBackbufferOffset();
        }

        public static global::Uno.Recti GetBackbufferScissor(GraphicsContextHandle handle)
        {
            return (handle).GetBackbufferScissor();
        }

        public static int GetRealBackbufferHeight(GraphicsContextHandle handle)
        {
            return (handle).GetRealBackbufferHeight();
        }
    }
}
