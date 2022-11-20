using System.Runtime.Versioning;
using OpenGL;
using Uno.Platform;

namespace Uno.AppLoader.MonoMac
{
    [SupportedOSPlatform("macOS10.14")]
    class MonoMacGraphicsContext : GraphicsContextBackend
    {
        UnoGLView _view;

        public MonoMacGraphicsContext(UnoGLView view)
        {
            _view = view;
        }

        public override GLFramebufferHandle GetBackbufferGLHandle()
        {
            return new GLFramebufferHandle(0);
        }

        public override Int2 GetBackbufferSize()
        {
            return _view.DrawableSize;
        }

        public override Int2 GetBackbufferOffset()
        {
            return new Int2(0, 0);
        }

        public override Recti GetBackbufferScissor()
        {
            return new Recti(new Int2(0, 0), _view.DrawableSize);
        }

        public override int GetRealBackbufferHeight()
        {
            return _view.DrawableSize.Y;
        }
    }
}

