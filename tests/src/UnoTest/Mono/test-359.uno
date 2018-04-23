namespace Mono.test_359
{
    partial interface B {
      void foo ();
    }
    
    partial interface B {
      void faa ();
    }
    
    public class Test
    {
        [Uno.Testing.Test] public static void test_359() { Main(); }
        public static void Main()
        {
        }
    }
}
