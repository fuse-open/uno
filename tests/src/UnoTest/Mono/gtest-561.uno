namespace Mono.gtest_561
{
    // Compiler options: -r:gtest-561-lib.dll
    
    using Uno.Collections;
    
    class C : A, I
    {
        public void Foo<T> (List<T> arg) where T : A
        {
        }
        
        [Uno.Testing.Test] public static void gtest_561() { Main(); }
        public static void Main()
        {
            new C ().Foo(new List<A> ());
        }
    }
}
