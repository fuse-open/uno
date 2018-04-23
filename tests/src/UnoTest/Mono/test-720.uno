namespace Mono.test_720
{
    // Compiler options: -warn:4 -warnaserror
    
    using Uno;
    
    namespace N
    {
        class Program
        {
            [Uno.Testing.Test] public static void test_720() { Main(); }
        public static void Main()
            {
                Parent pr = new Child();
                ((Child)pr).OnExample();
            }
        }
    
        public abstract class Parent
        {
            public delegate void ExampleHandler();
            public abstract event ExampleHandler Example;
        }
    
        public class Child : Parent
        {
            public override event ExampleHandler Example;
            public void OnExample()
            {
                if (Example != null) Example();
            }
        }
    }
}
