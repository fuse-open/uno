using GL = OpenTK.Graphics.OpenGL.GL;

namespace Uno.AppLoader.MonoMac
{
    class FramebufferDisposable : IContextObjectDisposable
    {
        public int HandleName { get; private set; }

        public FramebufferDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            var handle = HandleName;
            GL.DeleteFramebuffers(1, ref handle);
        }
    }
}