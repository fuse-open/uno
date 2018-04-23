using OpenGL;
using Uno.Compiler.ExportTargetInterop;
using Uno.Graphics;

namespace Uno.Runtime.Implementation
{
    [TargetSpecificType]
    [extern(DOTNET) DotNetType]
    [extern(CPLUSPLUS && !MOBILE) Set("TypeName", "uGraphicsContext")]
    [extern(CPLUSPLUS && !MOBILE) Set("Include", "Uno/GraphicsContext.h")]
    [extern(CPLUSPLUS && MOBILE) Set("TypeName", "void*")]
    struct GraphicsContextHandle
    {
    }

    [extern(DOTNET) DotNetType]
    [extern(CPLUSPLUS && !MOBILE) Require("Header.Include", "Uno/GraphicsContext.h")]
    static extern(!MOBILE) class GraphicsControllerImpl
    {
        public static GraphicsContextHandle GetInstance()
        {
            if defined(CPLUSPLUS)
            @{
                return uGraphicsContext::GetInstance();
            @}
            else if defined(CSHARP)
            @{
                return global::Uno.ApplicationContext.AppHost.GetGraphicsContext();
            @}
            else if defined(JAVASCRIPT)
            @{
                return {};
            @}
            else
                build_error;
        }

        extern(OPENGL)
        public static GLFramebufferHandle GetBackbufferGLHandle(GraphicsContextHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return $0.GetBackbufferGLHandle();
            @}
            else if defined(CSHARP)
            @{
                return ($0).GetBackbufferGLHandle();
            @}
            else if defined(JAVASCRIPT)
            @{
                return null;
            @}
            else
                build_error;
        }

        public static int2 GetBackbufferSize(GraphicsContextHandle handle)
        {
            if defined(CPLUSPLUS)
            @{
                return $0.GetBackbufferSize();
            @}
            else if defined(CSHARP)
            @{
                return ($0).GetBackbufferSize();
            @}
            else if defined(JAVASCRIPT)
            @{
                return @{int2(int,int):New(canvas.width, canvas.height)};
            @}
            else
                build_error;
        }

        public static int2 GetBackbufferOffset(GraphicsContextHandle handle)
        {
            if defined(CSHARP)
            @{
                return ($0).GetBackbufferOffset();
            @}
            else
                return int2(0, 0);
        }

        public static Recti GetBackbufferScissor(GraphicsContextHandle handle)
        {
            if defined(CSHARP)
            @{
                return ($0).GetBackbufferScissor();
            @}
            else
                return new Recti(GetBackbufferOffset(handle), GetBackbufferSize(handle));
        }

        public static int GetRealBackbufferHeight(GraphicsContextHandle handle)
        {
            if defined(CSHARP)
            @{
                return ($0).GetRealBackbufferHeight();
            @}
            else
                return GetBackbufferSize(handle).Y;
        }
    }
}
