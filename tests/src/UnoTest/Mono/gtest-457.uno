namespace Mono.gtest_457
{
    using Uno;
    
    class Program
    {
        class C
        {
        }
    
        void Foo<T> () where T : C
        {
        }
    
        [Uno.Testing.Test] public static void gtest_457() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return 0;
        }
    }
}
