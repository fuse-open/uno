namespace Mono.test_111
{
    class T {
            static object get_obj() {
                    return new object ();
            }
            [Uno.Testing.Test] public static void test_111() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
                    object o = get_obj ();
                    if (o == "string")
                            return 1;
                    return 0;
            }
    }
}
