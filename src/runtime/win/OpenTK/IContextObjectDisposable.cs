namespace Uno.AppLoader.OpenTK
{
    interface IContextObjectDisposable
    {
        int HandleName { get; }

        void Dispose();
    }
}