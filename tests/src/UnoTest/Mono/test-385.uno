namespace Mono.test_385
{
    class Test {
            [Uno.Testing.Test] public static void test_385() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
            {
                    int i = 5;
                    switch (i) {
                    case 5:
                            if (i == 5)
                                    break;
                            return 1;
                    default:
                            return 2;
                    }
                    Console.WriteLine (i);
            return 0;
            }
    }
}
