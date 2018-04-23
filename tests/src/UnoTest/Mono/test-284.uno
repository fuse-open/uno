namespace Mono.test_284
{
    public class App
    {
        [Uno.Testing.Test] public static void test_284() { Main(); }
        public static void Main()
        {
        object a = uint.MaxValue - ushort.MaxValue;
        }
    }
}
