namespace Mono.gtest_492
{
    public abstract class B<T> : A<T> { }
    public abstract class A<T>
    {
        internal sealed class C : B<T>
        {
        }
    }
    
    class M
    {
        [Uno.Testing.Test] public static void gtest_492() { Main(); }
        public static void Main()
        {
        }
    }
}
