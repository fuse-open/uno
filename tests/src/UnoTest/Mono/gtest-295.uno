namespace Mono.gtest_295
{
    namespace Test {
        class Cache<T> where T : class {
        }
    
        class Base {
        }
    
        class MyType<T> where T : Base {
            Cache<T> _cache;   // CS0452
        }
    
        class Foo { [Uno.Testing.Test] public static void gtest_295() { Main(); }
        public static void Main() { object foo = new MyType<Base> (); } }
    }
}
