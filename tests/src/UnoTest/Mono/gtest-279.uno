namespace Mono.gtest_279
{
    using Uno;
    using Uno.Collections;
    
    interface IFoo
    {
        void Bar();
        IList<T> Bar<T>();
    }
    
    class Foo : IFoo
    {
        public void Bar()
        {
            Console.WriteLine("Bar");
        }
        
        public IList<T> Bar<T>()
        {
            Console.WriteLine("Bar<T>");
            return null;
        }
    }
    
    class BugReport
    {
        [Uno.Testing.Test] public static void gtest_279() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            Foo f = new Foo();
            f.Bar();
            f.Bar<int>();
        }
    }
}
