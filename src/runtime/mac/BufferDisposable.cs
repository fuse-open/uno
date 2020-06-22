using GL = OpenTK.Graphics.OpenGL.GL;

namespace Uno.AppLoader.MonoMac
{
    class BufferDisposable : IContextObjectDisposable
    {
        public int HandleName { get; private set; }

        public BufferDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            var handle = HandleName;
            GL.DeleteBuffers(1, ref handle);
        }
    }
}