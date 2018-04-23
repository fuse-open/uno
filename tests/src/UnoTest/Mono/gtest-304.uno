namespace Mono.gtest_304
{
    //
    // Second test from bug 80518
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
        public abstract void Print<T>(object x) where T: BaseClass;
        } 
        
        public class PrinterImpl: Printer
        {
        public override void Print<T>(object x)
        {
            Console.WriteLine((x as T).Hello);
        }
        }
    
        public class Starter
        {
        [Uno.Testing.Test] public static void gtest_304() { Main(new string[0]); }
        public static void Main(string[] args )
        {
            BaseClass bc = new BaseClass();
            Printer p = new PrinterImpl();
            p.Print<BaseClass>(bc);
        }    
        }
    }
}
