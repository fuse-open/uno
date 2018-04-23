namespace Mono.gtest_autoproperty_01
{
    // Tests automatic properties
    using Uno;
    
    public class Test
    {
        private class A
        {
            public string B { get; set; }
        }
        
        public string Foo { get; set; }
        public int Answer { get; private set; }
        
        public Test ()
        {
            Answer = 42;
        }
        
        [Uno.Testing.Test] public static void gtest_autoproperty_01() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Test t = new Test ();
            t.Foo = "Bar";
            if (t.Foo != "Bar")
                return 1;
            
            if (t.Answer != 42)
                return 2;
            
            A a = new A ();
            a.B = "C";
            if (a.B != "C")
                return 3;
            
            return 0;
        }
    }
}
