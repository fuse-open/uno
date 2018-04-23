namespace Mono.gtest_313
{
    using Uno;
    
    class Foo<T>
    {
        public enum TestEnum { One, Two, Three }
    
        public TestEnum Test;
    
        public Foo (TestEnum test)
        {
            this.Test = test;
        }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_313() { Main(); }
        public static void Main()
        {
            Foo<int>.TestEnum e = Foo<int>.TestEnum.One;
            Console.WriteLine (e);
    
            Foo<int> foo = new Foo<int> (e);
            foo.Test = e;
        }
    }
}
