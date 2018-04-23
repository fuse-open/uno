namespace Mono.test_274
{
    public class MyClass {
            public MyClass( params Uno.String[] parameters )
            {
            }
    }
    
    public class ChildClass : MyClass {}
        
    public class M
    {
        [Uno.Testing.Test] public static void test_274() { Main(); }
        public static void Main()
        {
        }
    }
}
