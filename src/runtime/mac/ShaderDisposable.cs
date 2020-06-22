using GL = OpenTK.Graphics.OpenGL.GL;

namespace Uno.AppLoader.MonoMac
{
    class ShaderDisposable : IContextObjectDisposable
    {
        public int HandleName { get; private set; }

        public ShaderDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            GL.DeleteShader(HandleName);
        }
    }
}