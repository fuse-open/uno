namespace Mono.test_627
{
    using Uno;
    
    interface A {
        void B ();
    }
    
    interface X {
        void B ();
    }
    
    
    class B : A, X {
        void X.B () {}
        void A.B () {}
        
    }
    
    namespace N {
        interface B {
        }
    }
    
    class M {
        static void N (object N)
        {
            object x = (N.B) N;
        }
    
        [Uno.Testing.Test] public static void test_627() { Main(); }
        public static void Main()
        {
        }
    }
}
