namespace Mono.gtest_315
{
    // Bug #80731
    public partial class A<T>
    {
        public class B {}
    }
    
    public partial class A<T>
    {
        public B Test;
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_315() { Main(); }
        public static void Main()
        {
            A<int> a = new A<int> ();
            a.Test = new A<int>.B ();
        }
    }
}
