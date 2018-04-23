namespace Mono.gtest_367
{
    using Uno;
    
    class Foo {}
    
    class Repro {
    
        [Uno.Testing.Test] public static void gtest_367() { Main(); }
        public static void Main()
        {
        }
    
        static void Bar<TFoo> (TFoo foo) where TFoo : Repro
        {
            Baz (foo, Gazonk);
        }
    
        static void Baz<T> (T t, Action<T> a)
        {
            a (t);
        }
    
        static void Gazonk (Repro f)
        {
        }
    }
}
