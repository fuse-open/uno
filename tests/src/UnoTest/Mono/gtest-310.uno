namespace Mono.gtest_310
{
    using Uno;
    using Uno.Collections;
    
    namespace MonoBugs
    {
        public static class IncompleteGenericInference
        {
            public static void DoSomethingGeneric<T1, T2>(IEnumerable<T1> t1, IDictionary<T1,T2> t2)
            {
            }
    
            [Uno.Testing.Test] public static void gtest_310() { Main(); }
        public static void Main()
            {
                List<int> list = new List<int>();
                Uno.Collections.Dictionary<int, float> dictionary = new Dictionary<int, float>();
                DoSomethingGeneric(list, dictionary);
            }
        }
    }
}
