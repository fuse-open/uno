namespace Mono.test_879
{
    struct AStruct
    {
        public object foo;
    
        public AStruct (int i)
            : this ()
        {
        }
    }
    
    public class Tests
    {
        [Uno.Testing.Test] public static void test_879() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            for (int i = 0; i < 100; ++i) {
                AStruct a;
    
                a = new AStruct (5);
                if (a.foo != null)
                    return 1;
    
                a.foo = i + 1;
            }
    
            Console.WriteLine ("ok");
            return 0;
        }
    }
}
