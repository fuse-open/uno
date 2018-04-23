namespace Mono.test_103
{
    //
    // We should also allow overrides to work on protected methods.
    // Only private is not considered part of the override process.
    //
    abstract class A {
            protected abstract int Foo ();
    }
    
    class B : A {
            protected override int Foo ()
        {
            return 0;
        }
    
        public int M ()
        {
            return Foo ();
        }
    }
    
    class Test {
            [Uno.Testing.Test] public static void test_103() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return new B ().M ();
            }
    }
}
