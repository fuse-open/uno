namespace Uno.Support.OpenTK
{
    class RenderbufferDisposable : IContextObjectDisposable
    {
        public int HandleName { get; }

        public RenderbufferDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            global::OpenTK.Graphics.ES20.GL.DeleteRenderbuffer(HandleName);
        }
    }
}