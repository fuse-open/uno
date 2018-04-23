namespace Mono.gtest_optional_32
{
    using Uno;
    
    abstract class A
    {
        public abstract int[] Foo (params int[] args);
    }
    
    class B : A
    {
        public override int[] Foo (int[] args = null)
        {
            return args;
        }
    
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void gtest_optional_32() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var b = new B();
            if (b.Foo().Length != 0)
                return 1;
    
            return 0;
        }
    }
}
