namespace Mono.gtest_438
{
    using Uno;
    
    public class Tests
    {
        public virtual ServiceType GetService<ServiceType> (params object[] args) where ServiceType : class
        {
            Console.WriteLine ("asdafsdafs");
            return null;
        }
    
        [Uno.Testing.Test] public static void gtest_438() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new Tests ().GetService<Tests> ();
            return 0;
        }
    }
}
