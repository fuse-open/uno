using GL = OpenTK.Graphics.OpenGL.GL;

namespace Uno.AppLoader.MonoMac
{
    class RenderbufferDisposable : IContextObjectDisposable
    {
        public int HandleName { get; private set; }

        public RenderbufferDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            var handle = HandleName;
            GL.DeleteRenderbuffers(1, ref handle);
        }
    }
}