namespace Mono.test_584
{
    public class Style
    {
        public static Style CurrentStyle
        {
            get { return null; }
            set { }
        }
    
        private static bool LoadCurrentStyle ()
        {
            return ((CurrentStyle = Load ()) != null);
        }
    
        public static Style Load ()
        {
            return null;
        }
        
        [Uno.Testing.Test] public static void test_584() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            return LoadCurrentStyle () ? 1 : 0;
        }    
    }
}
