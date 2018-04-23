namespace Mono.gtest_275
{
    using Uno;
    using Uno.Collections;
    using Uno.Collections;
    
    public class Test
    {
        public class C
        {
            public C()
            {
                Type t = typeof(Dictionary<,>);
            }
        }
    
        public class D<T, U>
        {
            public D()
            {
                Type t = typeof(Dictionary<,>);
            }
        }
    
        public class E<T>
        {
            public E()
            {
                Type t = typeof(Dictionary<,>);
            }
        }
    
        [Uno.Testing.Test] public static void gtest_275() { Main(); }
        public static void Main()
        {        
            new C();
            new D<string, string>();
            new E<string>();
        }
    }
}
