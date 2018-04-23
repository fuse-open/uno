namespace Mono.test_495
{
    // This code must be compilable without any warning
    // Compiler options: -warnaserror -warn:4
    
    using Uno;
    
    namespace plj
    {
        public abstract class aClass
        {
            public static implicit operator aClass(fromClass o)
            { 
                return null;
            }
        }
        
        public class realClass1 : aClass
        {
            public static implicit operator realClass1(fromClass o)
            {
                return null;
            }
        }
        
        public class fromClass
        {
            [Uno.Testing.Test] public static void test_495() { Main(); }
        public static void Main() {}
        }
    }
}
