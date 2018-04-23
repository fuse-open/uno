namespace Mono.test_73
{
    //
    // This test is used to test that we do not use the .override
    // command on abstract method implementations.
    //
    
    public abstract class Abstract {
        public abstract int A ();
    }
    
    public class Concrete : Abstract {
        public override int A () {
            return 1;
        }
    }
    
    class Test {
    
        [Uno.Testing.Test] public static void test_73() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Concrete c = new Concrete ();
    
            if (c.A () != 1)
                return 1;
    
            return 0;
        }
    }
}
