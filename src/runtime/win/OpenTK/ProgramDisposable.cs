namespace Uno.Support.OpenTK
{
    class ProgramDisposable : IContextObjectDisposable
    {
        public int HandleName { get; }

        public ProgramDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            global::OpenTK.Graphics.ES20.GL.DeleteProgram(HandleName);
        }
    }
}