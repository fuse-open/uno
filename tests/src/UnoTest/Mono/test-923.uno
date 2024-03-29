namespace Mono.test_923
{
    using Uno;

    public struct Location
    {
        public int x;
        public int y;
    }

    public struct LocationWrapper
    {
        public Location location;
    }

    class Program
    {
        [Uno.Testing.Test] public static void test_923() { Main(); }
        public static void Main()
        {
        }

        public static void Test (out Location location)
        {
            location.x = 0;
            location.y = location.x;
        }

        public static void Test (LocationWrapper member)
        {
            member.location.x = 0;
            member.location.y = member.location.x;
        }

        public static void Test (out LocationWrapper member)
        {
            member.location.x = 0;
            member.location.y = member.location.x;
        }
    }
}
