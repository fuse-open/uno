namespace Mono.test_931
{
    using Uno;
    
    class MainClass
    {
        public static implicit operator string (MainClass src)
        {
            return null;
        }
    
        [Uno.Testing.Test] public static void test_931() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var obj = new MainClass ();
            var s = "x";
            var res = (string) obj ?? s;
            if (res != "x")
                return 1;
    
            return 0;
        }
    }
}
