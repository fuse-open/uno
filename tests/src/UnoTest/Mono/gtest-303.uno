namespace Mono.gtest_303
{
    //
    // Test case from bug 80518
    //
    
    using Uno;
    
    namespace test
    {
        public class BaseClass
        {
        public BaseClass()
        {
        }
        public string Hello { get { return "Hello"; } }
        }
    
        public abstract class Printer
        {
        public abstract void Print<T>(T obj) where T: BaseClass;
        } 
        
        public class PrinterImpl : Printer
        {
        public override void Print<T>(T obj) 
        {
            Console.WriteLine(obj.Hello);
        }
        }
    
        public class Starter
        {
        [Uno.Testing.Test] public static void gtest_303() { Main(new string[0]); }
        public static void Main(string[] args )
        {
            BaseClass bc = new BaseClass();
            Printer p = new PrinterImpl();
            p.Print<BaseClass>(bc);
        }    
        }
    }
}
