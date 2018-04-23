namespace Mono.test_726
{
    interface IFoo
    {
        object Clone ();
    }
    
    class CS0102 : IFoo
    {
        object IFoo.Clone()
        {
            return this;
        }
    
        public class Clone { }
    
        [Uno.Testing.Test] public static void test_726() { Main(); }
        public static void Main()
        {
        }
    }
}
