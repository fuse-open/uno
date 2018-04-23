namespace Mono.test_415
{
    // Compiler options: -r:test-415-lib.dll
    
    using Uno;
    public class MyTestExtended : MyTestAbstract
    {
        public MyTestExtended() : base()
        {
        }
    
        protected override string GetName() { return "foo"; }
        [Uno.Testing.Test] public static void test_415() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            Console.WriteLine("Calling PrintName");
            MyTestExtended test = new MyTestExtended();
            test.PrintName();
            Console.WriteLine("Out of PrintName");
        }
        
    }
}
