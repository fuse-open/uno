namespace Mono.test_357
{
    namespace SD {
        public class Sd {
            static public void F (bool b) { }
        }
    }
    
    namespace Foo {
        using SD;
        partial class Bar {
            delegate void f_t (bool b);
            f_t f = new f_t (Sd.F);
        }
    }
    
    namespace Foo {
        partial class Bar
        {
            public Bar () {}
            [Uno.Testing.Test] public static void test_357() { Main(); }
        public static void Main()
            {
                if (new Bar ().f == null)
                    throw new Uno.Exception ("Didn't resolve Sd.F?");
            }
        }
    }
}
