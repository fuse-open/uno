namespace Mono.gtest_203
{
    class C<X,Y> {
      class Q<A,B> {
        public void @apply (C<X,Y> t)
            {
              t.bar<A,B>();
            }
      }
    
      public void foo<A,B> ()
      {
        Q<A,B> q = new Q<A,B>();
            q.@apply(this);
      }
    
      public void bar<A,B> ()
      {
        Console.WriteLine ("'{0} {1} {2} {3}'",
    typeof(X),typeof(Y),typeof(A),typeof(B));
      }
    }
    
    class X {
      [Uno.Testing.Test] public static void gtest_203() { Main(); }
        public static void Main() {
        C<int,string> c = new C<int,string>();
            c.foo<float,string> ();
      }
    }
}
