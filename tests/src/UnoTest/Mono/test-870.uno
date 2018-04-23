namespace Mono.test_870
{
    public class Test
    {
        static void Foo (ushort p)
        {
            p = 0x0000;
            p |= 0x0000;
            p &= 0x0000;
    
            const ushort c = 0x0000;
            p &= c;
        }
    
        [Uno.Testing.Test] public static void test_870() { Main(); }
        public static void Main()
        {
            Foo (1);
        }
    }
}
