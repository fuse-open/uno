namespace Mono.gtest_170
{
    class C <A> {
      public static void foo<B> (C<B> x)
      {
        D.append (x);
      }
    }
    
    class D {
      public static void append<A> (C<A> x)
      {
      }
    
      [Uno.Testing.Test] public static void gtest_170() { Main(); }
        public static void Main()
      {
        C<object>.foo<int> (null);
      }
    }
}
