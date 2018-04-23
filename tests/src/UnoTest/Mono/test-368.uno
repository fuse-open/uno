namespace Mono.test_368
{
    class X {
    
        [Uno.Testing.Test] public static void test_368() { Main(); }
        public static void Main()
        {
            int n = 0;
            
            try {
            } finally {
                switch (n){
                case 0:
                    break;
                }
            }
        }
    }
}
