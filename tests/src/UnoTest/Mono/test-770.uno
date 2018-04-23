namespace Mono.test_770
{
    using Uno;
    
    public class MyClass
    {
        public class A
        {
            public event EventHandler MouseClick;
        }
    
        public class B : A
        {
            public new event EventHandler MouseClick;
        }
    
        public class C : B
        {
            public new void MouseClick ()
            {
                Console.WriteLine ("This should be printed");
            }
        }
    
        [Uno.Testing.Test] public static void test_770() { Main(); }
        public static void Main()
        {
            C myclass = new C ();
            myclass.MouseClick ();
        }
    }
}
