namespace Mono.test_203
{
    public enum Modifiers
    {
        Public = 0x0001
    }
    
    class Foo
    {
        internal Modifiers Modifiers {
            get {
                return Modifiers.Public;
            }
        }
    }
    
    class Bar
    {
        [Uno.Testing.Test] public static void test_203() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Console.WriteLine (Modifiers.Public);
            return 0;
        }
    }
}
