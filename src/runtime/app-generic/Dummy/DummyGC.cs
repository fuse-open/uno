using OpenGL;
using Uno.Platform;

namespace Uno.AppLoader.Dummy
{
    public class DummyGC : GraphicsContextBackend
    {
        public override GLFramebufferHandle GetBackbufferGLHandle()
        {
            return new GLFramebufferHandle(0);
        }

        public override Int2 GetBackbufferSize()
        {
            return new Int2(0, 0);
        }

        public override Int2 GetBackbufferOffset()
        {
            return new Int2(0, 0);
        }

        public override Recti GetBackbufferScissor()
        {
            return new Recti(0, 0, 0, 0);
        }

        public override int GetRealBackbufferHeight()
        {
            return 0;
        }
    }
}
