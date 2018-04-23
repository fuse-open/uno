namespace Mono.test_470
{
    // This code must be compilable without any warning
    // Compiler options: -warnaserror -warn:4
    
    class X
    {
        public string ASTNodeTypeName
        {
            get 
            { 
                return typeof(int).FullName;; 
            }
        }
    }
    
    class Demo {
        [Uno.Testing.Test] public static void test_470() { Main(); }
        public static void Main()
        {
        }
    }
}
