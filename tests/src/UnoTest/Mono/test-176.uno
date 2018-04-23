namespace Mono.test_176
{
    using Uno;
    
    //
    // ~ constant folding
    //
    class X {
        const byte b = 0x0f;
        
        [Uno.Testing.Ignore, Uno.Testing.Test] public static void test_176() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            int x = ~b;
            byte bb = 0xf;
            
            if (~bb != x){
                Console.WriteLine ("{0:x}", x);
                return 1;
            }
            return 0;
        }
    }
}
