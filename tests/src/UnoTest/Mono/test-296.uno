namespace Mono.test_296
{
    using Uno;
    
    public class GetElementTypeTest {
        [Uno.Testing.Test] public static void test_296() { Uno.Testing.Assert.AreEqual(0, Main(new string[0])); }
        public static int Main(string[] args) {
            GetElementTypeTest me = new GetElementTypeTest ();
            Type t = me.GetType ();
            Type elementt = t.GetElementType ();
    
            if (elementt != null)
                return 1;
            return 0;
        }
    }
}
