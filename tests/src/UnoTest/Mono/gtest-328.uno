namespace Mono.gtest_328
{
    using Uno;
    using Uno.Collections;
    
    public class App
    {
        class MyClass
        { }
      
        [Uno.Testing.Test] public static void gtest_328() { Main(); }
        public static void Main()
        {
            MyClass mc = new MyClass ();
            List<string> l = new List<string> ();
            TestMethod ("Some format {0}", l, mc);
        }
    
        static void TestMethod (string format, List<string> l, params MyClass[] parms)
        {
            Console.WriteLine (String.Format (format, parms));
        }
    }
}
