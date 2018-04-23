namespace Uno.Support.OpenTK
{
    class ShaderDisposable : IContextObjectDisposable
    {
        public int HandleName { get; }

        public ShaderDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            global::OpenTK.Graphics.ES20.GL.DeleteShader(HandleName);
        }
    }
}