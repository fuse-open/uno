namespace Uno.Runtime.Implementation.Internal
{
    [Obsolete("Please use Uno.Platform.Internal.Unsafe instead")]
    public static class Unsafe
    {
        public static void Quit()
        {
            Platform.Internal.Unsafe.Quit();
        }
    }
}
