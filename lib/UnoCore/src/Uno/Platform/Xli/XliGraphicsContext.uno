using Uno.Compiler.ExportTargetInterop;
using OpenGL;

namespace Uno.Platform.Xli
{
    [TargetSpecificType]
    [Set("TypeName", "uGraphicsContext")]
    [Set("Include", "Uno/GraphicsContext.h")]
    extern(CPLUSPLUS && !MOBILE) struct XliGraphicsContextPtr
    {
    }

    [Require("Header.Include", "Uno/GraphicsContext.h")]
    extern(CPLUSPLUS && !MOBILE) class XliGraphicsContext : GraphicsContextBackend
    {
        readonly XliGraphicsContextPtr _ptr;

        public XliGraphicsContext()
        @{
            @{$$._ptr} = uGraphicsContext::GetInstance();
        @}

        extern(OPENGL)
        public override GLFramebufferHandle GetBackbufferGLHandle()
        @{
            return @{$$._ptr}.GetBackbufferGLHandle();
        @}

        public override int2 GetBackbufferSize()
        @{
            return @{$$._ptr}.GetBackbufferSize();
        @}

        public override int2 GetBackbufferOffset()
        {
            return int2(0, 0);
        }

        public override Recti GetBackbufferScissor()
        {
            return new Recti(GetBackbufferOffset(), GetBackbufferSize());
        }

        public override int GetRealBackbufferHeight()
        {
            return GetBackbufferSize().Y;
        }
    }
}
