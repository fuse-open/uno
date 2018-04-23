namespace Mono.test_139
{
    //
    // This tests two uses of the `This' expression on structs; being used as an argument
    // and being used implicitly.
    //
    
    struct T {
            int val;
            void one () {
    
            //
            // First test: Pass this as an argument.
            //
                    two (this);
            }
    
            void two (T t)  {
            this = t;
            }
    
            void three (ref T t) {
                    two (t);
            }
    
    
            public override int GetHashCode () {
            //
            // Second test: do we correctly load this?
            //
                    return val.GetHashCode();
            }
    
            [Uno.Testing.Test] public static void test_139() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()     
        {
            T t = new T ();
    
            t.one ();
    
            t.GetHashCode ();
            
            return 0;
            }
    }
}
