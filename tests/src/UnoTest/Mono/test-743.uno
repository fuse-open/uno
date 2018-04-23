namespace Mono.test_743
{
    // Compiler options: -r:test-743-lib.dll
    
    using Uno;
    
    public class C : A
    {
        [Uno.Testing.Test] public static void test_743() { Main(); }
        public static void Main()
        {
            new C ().Test ();
        }
        
        void Test ()
        {
            var a = new C ();
            Console.WriteLine (a.Prop);
            a [5] = "2";
        }
    }
}
