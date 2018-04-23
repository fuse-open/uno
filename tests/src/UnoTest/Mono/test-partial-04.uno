namespace Mono.test_partial_04
{
    // Compiler options: -langversion:default
    
    namespace A
    {
        interface IFoo
        {
            void Hello (IFoo foo);
        }
    }
    
    namespace B
    {
        partial class Test
        { }
    }
    
    namespace B
    {
        using A;
    
        partial class Test : IFoo
        {
            void IFoo.Hello (IFoo foo)
            { }
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void test_partial_04() { Main(); }
        public static void Main()
        { }
    }
}
