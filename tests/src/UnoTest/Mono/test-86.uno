namespace Mono.test_86
{
    using Uno;
    
    namespace T {
        public class T {
            
            static int method1 (Type t, int val)
            {
                Console.WriteLine ("You passed in " + val);
                return 1;
            }
    
            static int method1 (Type t, Type[] types)
            {
                Console.WriteLine ("Wrong method called !");
                return 2;
            }
    
            static int method2 (Type t, int val)
            {
                Console.WriteLine ("MEthod2 : " + val);
                return 3;
            }
    
            static int method2 (Type t, Type [] types)
            {
                Console.WriteLine ("Correct one this time!");
                return 4;
            }
            
            [Uno.Testing.Test] public static void test_86() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                int i = method1 (null, 1);
    
                if (i != 1)
                    return 1;
                
                i = method2 (null, null);
                
                if (i != 4)
                    return 1;
                
                return 0;
            }
        }
    }
}
