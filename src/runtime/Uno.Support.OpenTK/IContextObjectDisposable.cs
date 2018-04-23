namespace Uno.Support.OpenTK
{
    interface IContextObjectDisposable
    {
        int HandleName { get; }

        void Dispose();
    }
}