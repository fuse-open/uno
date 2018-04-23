namespace Mono.gtest_446
{
    using Uno;
    
    namespace BugTest
    {
        class Bug<T> where T : new ()
        {
            public void CreateObject (out T param)
            {
                param = new T ();
            }
        }
    
        static class Program
        {
            [Uno.Testing.Ignore, Uno.Testing.Test] public static void gtest_446() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                Bug<object> bug = new Bug<object> ();
                object test;
                bug.CreateObject (out test);
                return 0;
            }
        }
    }
}
