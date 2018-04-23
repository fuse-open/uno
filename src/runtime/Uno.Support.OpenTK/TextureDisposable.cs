namespace Uno.Support.OpenTK
{
    class TextureDisposable : IContextObjectDisposable
    {
        public int HandleName { get; }

        public TextureDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            global::OpenTK.Graphics.ES20.GL.DeleteTexture(HandleName);
        }
    }
}