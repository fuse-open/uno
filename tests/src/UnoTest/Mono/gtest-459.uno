namespace Mono.gtest_459
{
    using Uno;
    
    namespace GenericTest
    {
        public class OuterGeneric<T>
        {
            public class InnerGeneric<U>
            {
                public static string GetTypeNames ()
                {
                    return typeof (T).ToString () + " " + typeof (U).ToString ();
                }
            }
        }
    
        class Program
        {
            [Uno.Testing.Test] public static void gtest_459() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                string typeNames = OuterGeneric<int>.InnerGeneric<long>.GetTypeNames ();
                Console.WriteLine (typeNames);
                return 0;
            }
        }
    }
}
