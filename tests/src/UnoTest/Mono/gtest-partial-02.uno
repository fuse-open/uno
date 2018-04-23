namespace Mono.gtest_partial_02
{
    partial class A<T>
    {
        void Test ()
        {
            this.CurrentItem = null;
        }
    }
    
    partial class A<T> where T : class
    {
        T CurrentItem { get; set; }
    }
    
    class C
    {
        [Uno.Testing.Test] public static void gtest_partial_02() { Main(); }
        public static void Main()
        {
        }
    }
}
