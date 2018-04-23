namespace Mono.test_122
{
    //
    // Tests that a nested class has full access to its container members
    //
    // A compile-only test.
    //
    
    class A {
            private static int X = 0;
    
            class B {
                    void Foo ()
                    {
                            ++ X;
                    }
            }
    
            [Uno.Testing.Test] public static void test_122() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
            return 0;
            }
    }
}
