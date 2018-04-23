namespace Mono.test_114
{
    using Uno;
    
    class MyClass {
    
        delegate bool IsAnything (Char c);
    
        [Uno.Testing.Test] public static void test_114() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
            IsAnything validDigit;
            validDigit = new IsAnything (Char.IsDigit);
    
            return 0;
        }
    }
}
