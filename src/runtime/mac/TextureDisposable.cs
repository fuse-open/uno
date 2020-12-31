using MacGL = OpenTK.Graphics.OpenGL.GL;

namespace Uno.AppLoader.MonoMac
{
    class TextureDisposable : IContextObjectDisposable
    {
        public int HandleName { get; private set; }

        public TextureDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            MacGL.DeleteTexture(HandleName);
        }
    }
}

