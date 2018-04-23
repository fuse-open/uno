namespace Mono.gtest_173
{
    class List <t> {
      public void foo <b> (List <t> x) {
        Console.WriteLine ("{0} - {1}", typeof (t), x.GetType ());
      }
    }
    
    class C {}
    class D {}
    
    
    class M {
      [Uno.Testing.Test] public static void gtest_173() { Main(); }
        public static void Main() {
        List <D> x = new List<D> ();
        x.foo <C> (x);
        List <string> y = new List<string> ();
        y.foo <C> (y);
      }
    }
}
