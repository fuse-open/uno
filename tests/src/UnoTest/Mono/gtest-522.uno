namespace Mono.gtest_522
{
    using Uno;
    
    class C<T>
    {
        public static int Foo;
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_522() { Main(); }
        public static void Main()
        {
        }
        
        void Test<T> (T A)
        {
            A<T> ();
            
            object C;
            var c = C<int>.Foo;
        }
        
        static void A<U> ()
        {
        }
    }
}
