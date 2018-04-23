namespace Mono.gtest_441
{
    using Uno.Collections;
    
    namespace Name
    {
        public class Test
        {
            internal static List<int> List;
        }
    
        public class Subclass : Test
        {
            private List<int> list;
    
            public List<int> List
            {
                get { return list; }
            }
    
            [Uno.Testing.Test] public static void gtest_441() { Main(new string[0]); }
        public static void Main(string[] args)
            {
                Subclass c = new Subclass ();
            }
        }
    }
}
