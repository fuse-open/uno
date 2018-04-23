namespace Mono.test_855
{
    namespace Test
    {
        public interface IInterface
        {
            string Get (string key, string v);
            int Get (string key, int v);
        }
    
        public class BaseClass
        {
            public string Get (string key, string v)
            {
                return v;
            }
    
            public int Get (string key, int v)
            {
                return 0;
            }
        }
    
        public class Subclass : BaseClass, IInterface
        {
            [Uno.Testing.Test] public static void test_855() { Main(); }
        public static void Main()
            {
                new Subclass ();
            }
        }
    }
}
