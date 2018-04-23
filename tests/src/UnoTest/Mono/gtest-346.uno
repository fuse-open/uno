namespace Mono.gtest_346
{
    public class test
    {
        public void CreateSimpleCallSite(int x) {}
        public void CreateSimpleCallSite<A>() {}
        public void CreateSimpleCallSite<A>(int x) {}
            
        [Uno.Testing.Test] public static void gtest_346() { Main(); }
        public static void Main()
        {
        }
    }
}
