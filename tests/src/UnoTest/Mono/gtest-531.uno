namespace Mono.gtest_531
{
    class ATop<T> : IA<T>
    {
        IA<T> list;
        
        T[] IB<T>.ToArray (T[] t)
        {
            return null;
        }
        
        void IC.ToArray ()
        {
        }
        
        public void Test ()
        {
            list = this;
            list.ToArray (new T [0]);
            list.ToArray ();
        }
    }
    
    interface IA<U> : IC, IB<U>
    {
    }
    
    interface IB<V> : IC
    {
        V[] ToArray (V[] array);
    }
    
    interface IC
    {
        void ToArray ();
    }
    
    class M
    {
        [Uno.Testing.Test] public static void gtest_531() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            new ATop<short>().Test ();
            return 0;
        }
    }
}
