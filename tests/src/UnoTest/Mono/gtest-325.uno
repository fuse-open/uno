namespace Mono.gtest_325
{
    public class SomeClass<T> where T : new() {
        public void Foo() {
            new T();
        }
    }
    
    class Foo {
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void gtest_325() { Main(); }
        public static void Main()
        {
            SomeClass<object> x = new SomeClass<object> ();
            x.Foo ();
        }
    }
}
