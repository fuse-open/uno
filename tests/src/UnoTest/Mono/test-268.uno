namespace Mono.test_268
{
    public enum MyEnum { V = 1 }
    
    class X {
        public MyEnum MyEnum;    
        class Nested {
            internal MyEnum D () { 
                return MyEnum.V; 
            }
        }
        
        [Uno.Testing.Test] public static void test_268() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
            Nested n = new Nested ();
            return n.D() == MyEnum.V ? 0 : 1;
        }
    }
}
