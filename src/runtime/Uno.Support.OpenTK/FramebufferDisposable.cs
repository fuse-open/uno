namespace Uno.Support.OpenTK
{
    class FramebufferDisposable : IContextObjectDisposable
    {
        public int HandleName { get; }

        public FramebufferDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            global::OpenTK.Graphics.ES20.GL.DeleteFramebuffer(HandleName);
        }
    }
}