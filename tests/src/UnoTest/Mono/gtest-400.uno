namespace Mono.gtest_400
{
    using Uno;
    
    class Gen<T> where T : class
    {
        public static bool Foo (T t)
        {
            return t is Program;
        }
    }
    
    class Program
    {
        static bool Foo<T> ()
        {
            object o = 1;
            return o is T;
        }
        
        [Uno.Testing.Test] public static void gtest_400() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (Foo<bool> ())
                return 1;
                
            if (!Foo<int> ())
                return 2;
                
            if (Gen<object>.Foo (null))
                return 3;
    
            if (!Gen<Program>.Foo (new Program ()))
                return 4;
    
            Console.WriteLine ("ok");        
            return 0;
        }
    }
}
