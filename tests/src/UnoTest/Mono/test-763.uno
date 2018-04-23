namespace Mono.test_763
{
    using Uno;
    
    public class StaticDelegateWithSameNameAsInstance
    {
        private Provider _provider;
        delegate void Provider (string s);
    
        Provider MyProvider
        {
            set
            {
                _provider = value;
                if (_provider != null) {
                    _provider ("v");
                }
            }
        }
    
        static int i = 1;
    
        public void StaticCallback ()
        {
            i += 7;
            MyProvider = StaticCallback;
        }
    
        public static void StaticCallback (string s)
        {
            if (s != "v")
                throw new ApplicationException ();
    
            i *= 3;
        }
    
        [Uno.Testing.Test] public static void test_763() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new StaticDelegateWithSameNameAsInstance ().StaticCallback ();
    
            Console.WriteLine (i);
            if (i != 24)
                return 1;
    
            return 0;
        }
    }
}
