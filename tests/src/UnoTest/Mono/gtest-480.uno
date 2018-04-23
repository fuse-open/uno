namespace Mono.gtest_480
{
    using Uno;
    using Uno.Collections;
    
    interface I<T> : ICollection<T>, IEnumerable<T>
    {
    }
    
    class C
    {
        void Foo ()
        {
            I<object> o = null;
            foreach (var v in o)
                Console.WriteLine (v);
        }
        
        [Uno.Testing.Test] public static void gtest_480() { Main(); }
        public static void Main()
        {
            IList<int> list = new List<int> { 1, 3 };
            var g = list.GetEnumerator ();
        }
    }
}
