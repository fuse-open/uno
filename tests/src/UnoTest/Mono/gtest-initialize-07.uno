namespace Mono.gtest_initialize_07
{
    public class A
    {
        public string Name { get; set; }
        
        public bool Matches (string s)
        {
            return Name == s;
        }
    }
    
    class M
    {
        [Uno.Testing.Test] public static void gtest_initialize_07() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (!new A () { Name = "Foo" }.Matches ("Foo"))
                return 1;
            
            return 0;
        }
    }
}
