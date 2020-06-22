namespace Uno.AppLoader.MonoMac
{
    interface IContextObjectDisposable
    {
        int HandleName { get; }

        void Dispose();
    }
}