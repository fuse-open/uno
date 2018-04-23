namespace Mono.gtest_484
{
    using Uno;
    
    class MainClass
    {
        static void Foo (params Action<MainClass>[][] funcs) { }
    
        static Action<MainClass>[] Set (params Action<MainClass>[] arr)
        {
            return arr;
        }
    
        static void Bar (MainClass mc) { }
    
        [Uno.Testing.Test] public static void gtest_484() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            Foo (Set (Bar, Bar), Set (Bar, Bar));
        }
    }
}
