namespace Mono.gtest_320
{
    // Bug #81019
    public class Foo<K>
    { }
    
    partial class B
    { }
    
    partial class B : Foo<B.C>
    {
        public class C
        { }
      
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_320() { Main(); }
        public static void Main()
        { }
    }
}
