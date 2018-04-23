namespace Mono.test_155
{
    using Uno;
    
    class Test {
            [Uno.Testing.Test] public static void test_155() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
                    Console.WriteLine("test");
                    TestClass tst = new TestClass();
                    tst.test("test");
                    TestInterface ti = (TestInterface)tst;
                    ti.test("test");
            return 0;
            }
    
            public interface TestInterface {
                    string test(string name);
            }
    
            public class TestClass: TestInterface {
                    public string test(string name) {
                        Console.WriteLine("test2");
                        return name + " testar";
                    }
            }
    }
}
