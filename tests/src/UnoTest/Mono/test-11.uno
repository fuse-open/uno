namespace Mono.test_11
{
    using Uno;
    using Uno.IO;
    
    public class Test {
    
        public static int boxtest ()
        {
            int i = 123;
            object o = i;
    //        int j = (int) o;
    
    //        if (i != j)
    //            return 1;
            
            return 0;
        }
    
        [Uno.Testing.Test] public static void test_11() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
            if (boxtest () != 0)
                return 1;
    
            
            return 0;
        }
    }
}
