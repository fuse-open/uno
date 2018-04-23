namespace Mono.test_537
{
    using Uno;
    class Base
    {
        protected string H {
            get {
                return "Base.H";
            }
        }
    }
    
    // #1
    class X {
        class Derived : Base
        {
            public class Nested : Base
            {
                public void G() {
                    Derived[] d = new Derived[0];
                    Console.WriteLine(d[0].H);
                }
            }
        }
    }
    
    // #2
    class Derived: Base
    {
        public class Nested : Base
        {
            public void G() {
                Derived d = new Derived();
                Console.WriteLine(d.H);
            }
        }
    }
    
    
    class Test
    {
        [Uno.Testing.Test] public static void test_537() { Main(); }
        public static void Main()
        {
        }
    }
}
