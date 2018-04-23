namespace Mono.gtest_104
{
    class MainClass
    {
            class Gen<T>
            {
            public void Test ()
            { }
            }
    
            class Der : Gen<int>
            {
            }
    
            [Uno.Testing.Test] public static void gtest_104() { Main(); }
        public static void Main()
            {
            object o = new Der ();
                    Gen<int> b = (Gen<int>) o;
            b.Test ();
            }
    }
}
