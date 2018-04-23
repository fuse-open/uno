namespace Mono.gtest_initialize_11
{
    using Uno;
    
    namespace InlineAssignmentTest
    {
        public class Foo
        {
            public bool B = true;
        }
    
        public class MainClass
        {
            [Uno.Testing.Test] public static void gtest_initialize_11() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                var foo = new Foo () { B = false };
                if (foo.B != false)
                    return 1;
    
                return 0;
            }
        }
    }
}
