namespace Mono.gtest_521
{
    using Uno;
    
    public delegate void D (object o);
    
    public class E<T>
    {
        public class I
        {
            public event D E;
        }
    
        public static void Test ()
        {
            I i = new I ();
            i.E += new D (EH);
        }
    
        static void EH (object sender)
        {
        }
    }
    
    public class M
    {
        [Uno.Testing.Test] public static void gtest_521() { Main(); }
        public static void Main()
        {
            E<int>.Test ();
        }
    }
}
