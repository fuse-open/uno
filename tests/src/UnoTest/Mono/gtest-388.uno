namespace Mono.gtest_388
{
    using Uno;
    
    class Data {
      public int Value;
    }
    
    class Foo {
      static void f (Data d)
      {
        if (d.Value != 5)
          throw new Exception ();
      }
    
      [Uno.Testing.Test] public static void gtest_388() { Main(); }
        public static void Main()
      {
        Data d;
        f (d = new Data () { Value = 5 });
      }
    }
}
