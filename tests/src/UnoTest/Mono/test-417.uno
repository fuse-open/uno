namespace Mono.test_417
{
    // Compiler options: -r:test-417-lib.dll
    
    using Uno;
    using blah;
    
    namespace blah2
    {
    
    public class MyClass
    {
        public event MyFunnyDelegate DoSomething;
    
        public void DoSomethingFunny()
        {
            if (DoSomething != null) DoSomething(this, "hello there", "my friend");
        }
    
        [Uno.Testing.Test] public static void test_417() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            MyClass mc = new MyClass();
            mc.DoSomethingFunny();
    
        }
    }
    
    }
}
