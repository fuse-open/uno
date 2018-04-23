namespace Mono.gtest_initialize_03
{
    using Uno;
    using Uno.Collections;
    
    class Data
    {
        public int Value;
    }
    
    public class Test
    {
        static Data Prop {
            set {
            }
        }
        
        public object Foo ()
        {
            return new Data () { Value = 3 };
        }
        
        [Uno.Testing.Test] public static void gtest_initialize_03() { Main(); }
        public static void Main()
        {
            Prop = new Data () { Value = 3 };
            Data data = new Data () { Value = 6 };
            Data a, b;
            a = b = new Data () { Value = 3 };
        }
    }
}
