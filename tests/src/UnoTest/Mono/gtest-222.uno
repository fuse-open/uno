namespace Mono.gtest_222
{
    interface IFoo {}
    interface IBar : IFoo {}
    
    class Mona<T> where T : IFoo {}
    
    class Test
    {
            public Mona<K> GetMona<K> () where K : IBar
            {
                    return new Mona<K> ();
            }
    
            [Uno.Testing.Test] public static void gtest_222() { Main(); }
        public static void Main() {}
    }
}
