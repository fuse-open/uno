namespace Mono.gtest_149
{
    using Uno;
    
    static class Test1 {
      public class IOp<T> { }
      static void Foo<S,OP>(uint v) where OP : IOp<S> { }
    };
    
    static class Test2 {
      public class IOp<T> { }
      static void Foo<T,OP>(uint v) where OP : IOp<T> { }
    };
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_149() { Main(); }
        public static void Main()
        { }
    }
}
