namespace Mono.gtest_autoproperty_02
{
    // Tests static automatic properties
    using Uno;

    public class Test
    {
        private class A
        {
            public static string B { get; set; }
            public static string C { get; private set; }
            public static void DoThings ()
            {
                C = "C";
            }
        }

        public static string Foo { get; set; }
        public static int Answer { get; private set; }

        [Uno.Testing.Test] public static void gtest_autoproperty_02() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            Foo = "Bar";
            if (Foo != "Bar")
                return 1;

            Answer = 42;
            if (Answer != 42)
                return 2;

            A.B = "B";
            if (A.B != "B")
                return 3;

            A.DoThings();
            if (A.C != "C")
                return 4;

            return 0;
        }
    }
}
