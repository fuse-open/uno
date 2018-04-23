namespace Mono.test_242
{
    // This code must be compilable without any warning
    // Compiler options: -warnaserror -warn:4
    
    class BaseClass {
            public int Location = 3;
    }
    
    class Derived : BaseClass {
        public new int Location {
                get {
                    return 9;
                }
            }
            
            [Uno.Testing.Test] public static void test_242() { Main(); }
        public static void Main() { }
    }
}
