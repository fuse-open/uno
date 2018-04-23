namespace Mono.gtest_288
{
    using Uno;
    
    public abstract class A
    {
        protected bool Test (int a)
        {
            return a == 5;
        }
    }
    
    public class B : A
    {
        public void Test ()
        { }
    
        class C : A
        {
            B b;
    
            public bool Foo (int a)
            {
                return b.Test (a);
            }
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_288() { Main(); }
        public static void Main()
        { }
    }
}
