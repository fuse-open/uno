namespace Mono.test_493
{
    class A {
        protected int f { get { return 1; } }
    }
    
    class B : A {
             int bar () { return new C().f; } 
       }
       
    class C : B {
        [Uno.Testing.Test] public static void test_493() { Main(); }
        public static void Main() {}
    }
}
