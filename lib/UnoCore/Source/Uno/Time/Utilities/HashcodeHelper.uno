namespace Uno.Time
{
    internal static class HashCodeHelper
    {
        private const int HashcodeMultiplier = 37;

        private const int HashcodeInitializer = 17;

        internal static int Initialize()
        {
            return HashcodeInitializer;
        }

        internal static int Hash<T>(int code, T value)
        {
            int hash = 0;
            if (!value.Equals(null))
            {
                hash = value.GetHashCode();
            }
            return MakeHash(code, hash);
        }

        private static int MakeHash(int code, int value)
        {
            return (code * HashcodeMultiplier) + value;
        }
    }
}
