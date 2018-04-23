namespace Mono.test_624
{
    using Uno;
    
    //
    // The problem here is that `(Type)' is being recognized as a Property
    // but inside a Cast expression this is invalid.
    //
    class X {
    
        int Type {
            get {
                return 1;
            }
        }
    
        [Uno.Testing.Test] public static void test_624() { Main(); }
        public static void Main()
        {
            Type t = (Type) null;
        }
    
    }
}
