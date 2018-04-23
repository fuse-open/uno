namespace Mono.gtest_initialize_08
{
    using Uno;
    using Uno.Collections;
    
    class T
    {
        public X[] x;
    }
    
    class X
    {
        public Z[] Prop { get; set; }
    }
    
    class Z
    {
    }
    
    class Test
    {
        T t = new T () { x = new X [] { 
            new X () {
                Prop = new Z[] { new Z (), new Z () }
            }
        }};
        
        public Test (string s)
        {
        }
        
        public Test (int i)
        {
        }
    }
    
    public class C
    {
        [Uno.Testing.Test] public static void gtest_initialize_08() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {        
            new Test ("2");
            return 0;
        }
    }
}
