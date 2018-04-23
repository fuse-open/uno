namespace Uno.Support.MonoMac
{
    interface IContextObjectDisposable
    {
        int HandleName { get; }

        void Dispose();
    }
}