using OpenGL;
using Uno.Platform;

namespace Uno.Support.WinForms
{
    public class WinFormsGraphicsContext : GraphicsContextBackend
    {
        UnoGLControl _control;

        public WinFormsGraphicsContext(UnoGLControl control)
        {
            _control = control;
        }

        public override GLFramebufferHandle GetBackbufferGLHandle()
        {
            return new GLFramebufferHandle(0);
        }

        public override Int2 GetBackbufferSize()
        {
            return new Int2(_control.ClientSize.Width, _control.ClientSize.Height);
        }

        public override Int2 GetBackbufferOffset()
        {
            return new Int2(0, 0);
        }

        public override Recti GetBackbufferScissor()
        {
            return new Recti(new Int2(0, 0), new Int2(_control.ClientSize.Width, _control.ClientSize.Height));
        }

        public override int GetRealBackbufferHeight()
        {
            return _control.ClientSize.Height;
        }
    }
}
