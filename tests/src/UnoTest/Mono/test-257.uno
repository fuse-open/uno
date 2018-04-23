namespace Mono.test_257
{
    class X {
            [Uno.Testing.Test] public static void test_257() { Main(); }
        public static void Main()
            {
                    int a;
    
                    call (out a);
            }
    
            static void call (out int a)
            {
                    while (true){
                            try {
                                    a = 1;
                                    return ;
                            } catch {
                            }
                    }
            }
    }
}
