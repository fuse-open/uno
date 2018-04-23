namespace Mono.gtest_189
{
    interface IFoo {}
    class Bar : IFoo {}
    
    class Cont<T> {
      T f;
      public Cont(T x) { f = x; }
      public override string ToString ()
      {
        return f.ToString ();
      }
    }
    
    class M {
      [Uno.Testing.Test] public static void gtest_189() { Main(); }
        public static void Main()
      {
        Cont<IFoo> c = new Cont<IFoo> (new Bar ());
        Console.WriteLine (c);
      }
    }
}
