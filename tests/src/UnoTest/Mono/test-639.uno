namespace Mono.test_639
{
    class Foo {
      bool got;
      string s {
        get { got = true; return ""; }
        set { if (!got || value != "A1B2") throw new Uno.Exception (); }
      }
    
      [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_639() { Main(); }
        public static void Main()
      {
        (new Foo ()).s += "A" + 1 + "B" + 2;
      }
    }
}
