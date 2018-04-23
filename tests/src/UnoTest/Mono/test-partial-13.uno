namespace Mono.test_partial_13
{
    namespace Test{  
        public partial class Test{  
        public override bool Equals(object obj){  
            return true;  
        }  
        }
        public partial class Test{  
        public override int GetHashCode(){  
            return 1;  
        }
        [Uno.Testing.Test] public static void test_partial_13() { Main(); }
        public static void Main() {}
        }  
    }
}
