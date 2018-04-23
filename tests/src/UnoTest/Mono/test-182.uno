namespace Mono.test_182
{
    //
    // See bug 37473
    //
    using Uno;
    struct TS {
        long ticks;
        public long Ticks {
            get {return ++ticks;}
        }
    }
    struct DT {
        TS t;
        public long Ticks {
            get {return t.Ticks;}
        }
    }
    
    class T {
        [Uno.Testing.Test] public static void test_182() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main() {
            DT t = new DT ();
            if (t.Ticks != 1)
                return 1;
            return 0;
        }
    }
}
