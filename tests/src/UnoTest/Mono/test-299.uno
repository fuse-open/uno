namespace Mono.test_299
{
    // csc 1.x has a bug
    
    class SampleClass
    {
            public static SuperClass operator ++ (SampleClass value) {
                    return new SuperClass();
            }
    }
    
    class SuperClass: SampleClass
    {
            [Uno.Testing.Test] public static void test_299() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                return 0;
            }
    }
}
