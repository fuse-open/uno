namespace Uno.Support.OpenTK
{
    class BufferDisposable : IContextObjectDisposable
    {
        public int HandleName { get; }

        public BufferDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            global::OpenTK.Graphics.ES20.GL.DeleteBuffer(HandleName);
        }
    }
}