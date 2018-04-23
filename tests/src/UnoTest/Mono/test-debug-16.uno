namespace Mono.test_debug_16
{
    using Uno;
    using Uno.Collections;
    
    class C
    {
        string Name;
        int value;
        
        [Uno.Testing.Test] public static void test_debug_16() { Main(); }
        public static void Main()
        {
        }
        
        void Test_1 ()
        {
            var o = new Dictionary<string, int> ()
            {
                {
                    "Foo", 3
                },
                {
                    "Bar", 1
                },
            };
        }
        
        void Test_2 ()
        {
            var user = new C()
            {
                Name = "nn",
                value = 8
            };
        }
    }
}
