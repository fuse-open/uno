namespace Mono.gtest_160
{
    class Fun<A,B> {}
    
    class List<T> {
      public List<T2> Map<T2> (Fun<T,T2> x)
      {
        return new List<T2>();
      }
    
      public void foo<T2> ()
      {
        (new List<T2> ()).Map<T> (new Fun<T2,T> ());
      }
    }
    
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_160() { Main(); }
        public static void Main()
        { }
    }
}
