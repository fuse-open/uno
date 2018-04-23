namespace Mono.gtest_182
{
    interface IMember {
      int GetId ();
    }
    
    interface IMethod : IMember { }
    
    class C1 : IMethod
    {
      public int GetId () { return 42; }
    }
    
    class X {
        static void foo<a> (a e )
          where a : IMember
        {
          e.GetId ();
        }
    
      [Uno.Testing.Test] public static void gtest_182() { Main(); }
        public static void Main()
      {
        foo<IMethod> (new C1 ());
      }
    }
}
