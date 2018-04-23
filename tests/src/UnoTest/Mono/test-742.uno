namespace Mono.test_742
{
    using Uno;
    
    public struct Test
    {
        public int Foo;
    
        public static Test Set (C c)
        {
            c.Value.Foo = 21;
            return c.Value;
        }
    }
    
    public class C
    {
        public Test Value;
    }
    public class Driver
    {
        [Uno.Testing.Test] public static void test_742() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var v = Test.Set (new C ());
            Console.WriteLine (v.Foo);
            if (v.Foo != 21)
                return 1;
            return 0;
        }
    }
}
