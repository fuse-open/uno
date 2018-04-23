namespace Mono.test_601
{
    using Uno;
    
    // Tests where keyword sensitivity
    
    class C
    {
        delegate void MarkerUpdatedVMDelegate (IntPtr buffer, IntPtr where);
        
        [Uno.Testing.Test] public static void test_601() { Main(); }
        public static void Main() {}
    }
}
