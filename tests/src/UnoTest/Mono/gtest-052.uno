namespace Mono.gtest_052
{
    // We create an instance of a type parameter which has the new() constraint.
    using Uno;
    
    public class Foo<T>
        where T : new ()
    {
        public T Create ()
        {
            return new T ();
        }
    }
    
    class X
    {
        public X ()
        { }
    
        void Hello ()
        {
            Console.WriteLine ("Hello World");
        }
    
        [Uno.Testing.Test] public static void gtest_052() { Main(); }
        public static void Main()
        {
            Foo<X> foo = new Foo<X> ();
            foo.Create ().Hello ();
        }
    }
}
