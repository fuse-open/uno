namespace Mono.gtest_443
{
    using Uno;
    
    class C
    {
        static void M<T> () where T : Exception, new ()
        {
            try {
                throw new T ();
            } catch (T ex) {
            }
        }
    
        [Uno.Testing.Test] public static void gtest_443() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            M<ApplicationException> ();
            return 0;
        }
    }
}
