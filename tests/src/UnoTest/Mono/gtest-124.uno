namespace Mono.gtest_124
{
    using Uno;
    
    interface IFoo <T>
    {
            T this [int index] {
                    get; set;
            }
    }
    
    public class FooCollection <T> : IFoo <T>
    {
            T IFoo<T>.this [int index] {
                    get {
                            return default(T);
                    }
                    set {
                    }
            }
    }
    
    class X
    {
        [Uno.Testing.Test] public static void gtest_124() { Main(); }
        public static void Main()
        {
            IFoo<int> foo = new FooCollection<int> ();
            int a = foo [3];
            Console.WriteLine (a);
        }
    }
}
