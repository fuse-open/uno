namespace Uno.AppLoader.MonoMac
{
    class ProgramDisposable : IContextObjectDisposable
    {
        public int HandleName { get; private set; }

        public ProgramDisposable(int handleName)
        {
            HandleName = handleName;
        }

        public void Dispose()
        {
            // OMG! MonoMac doesn�t expose GL.DeleteProgram(HandleName), and instead exposes a NV only extension...
            // A pull request will be made. For now let it be a memory leak.
            // Also MonoMac hasn�t released anything since 2011.
            //   -- Emil
            //MonoMac.OpenGL.GL.DeleteProgram(HandleName);
        }
    }
}