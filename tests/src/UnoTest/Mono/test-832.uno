namespace Mono.test_832
{
    public class A
    {
        [Uno.Testing.Test] public static void test_832() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            var a = new A ();
            a.Test ();
            if (a.Properties.P2.Value != 1)
                return 1;

            return 0;
        }

        S s = new S (55);

        void Test ()
        {
            Properties.P2.Value = 1;
        }

        internal S Properties {
            get { return s; }
        }
    }

    struct S
    {
        C c;

        public S (int i)
        {
            c = new C ();
        }

        public C P2
        {
            get { return c; }
        }
    }

    class C
    {
        public int Value;
    }
}
