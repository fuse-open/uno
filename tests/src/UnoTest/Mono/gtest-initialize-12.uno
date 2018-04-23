namespace Mono.gtest_initialize_12
{
    class C : B
    {
        public override int Foo { set {} }
    }
    
    abstract class B
    {
        public abstract int Foo { set; }
    }
    
    public class Test
    {
        static int Foo { set {} }
    
        [Uno.Testing.Test] public static void gtest_initialize_12() { Main(); }
        public static void Main()
        {
            var c = new C () { Foo = 1 };
        }
    }
}
