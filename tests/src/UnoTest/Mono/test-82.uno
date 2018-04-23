namespace Mono.test_82
{
    //
    // Test to ensure that we correctly perform type lookups - thanks to Felix A.I
    //
    namespace N1
    {    
        public enum A
        {
            A_1, A_2, A_3
        }
    
        namespace N2
        {    
            public class B
            {
                A member;
    
                void Method (ref A a)
                {
                }
    
                [Uno.Testing.Test] public static void test_82() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
                {
                    return 0;
                }
            }
    
        }
    }
    
    namespace N1.N3
    {    
        public class B
        {
            A member;
    
            void Method (ref A a)
            {
            }
        }
    }
}
