namespace Mono.gtest_026
{
    // Test access to class fields outside the generic type declaration.
    
    class Foo<T>
    {
        public T Hello;
    
        public Foo ()
        { }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_026() { Main(); }
        public static void Main()
        {
            Foo<int> foo = new Foo<int> ();
            foo.Hello = 9;
        }
    }
}
