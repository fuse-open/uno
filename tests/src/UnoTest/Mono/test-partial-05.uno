namespace Mono.test_partial_05
{
    // Compiler options: -langversion:default
    
    class B {
    }
    
    interface iface {
    }
    
    partial class A : B {
    }
    
    partial class A : iface {
    }
    
    
    partial class A2 : Uno.Object {
    }
    
    partial class A2 {
    }
    
    class D { [Uno.Testing.Test] public static void test_partial_05() { Main(); }
        public static void Main() {} }
}
