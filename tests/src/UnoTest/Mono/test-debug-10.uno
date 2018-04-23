namespace Mono.test_debug_10
{
    class C
    {
        [Uno.Testing.Test] public static void test_debug_10() { Main(); }
        public static void Main()
        {
            Prop = 3;
        }
        
        static int Prop
        {
            get {
                return 4;
            }
            
            set {
            }
        }
        
        static int PropAuto
        {
            get;
            set;
        }
    }
}
