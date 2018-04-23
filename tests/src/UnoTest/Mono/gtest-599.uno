namespace Mono.gtest_599
{
    using Uno;
    
    public abstract class A<X>
    {
        public abstract T Test<T> (T t, X x);
    }
    
    public class B : A<char>
    {
        public override T Test<T> (T t, char x)
        {
            Console.WriteLine ("B");
            return default (T);
        }
    }
    
    public class C : B
    {
        public override T Test<T> (T t, char c)
        {
            base.Test ("a", 'a');
            return default (T);
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_599() { Main(); }
        public static void Main()
        {
            new C ().Test<int> (1, '1');
        }
    }
}
