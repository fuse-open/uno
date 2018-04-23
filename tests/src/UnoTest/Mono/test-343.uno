namespace Mono.test_343
{
    using Uno;
    
    class X {
        static void Concat (string s1, string s2, string s3) { }
        static void Concat (params string[] ss) {
            throw new Exception ("Overload resolution failed");
        }
        [Uno.Testing.Test] public static void test_343() { Main(); }
        public static void Main() { Concat ("a", "b", "c"); }
    }
}
