namespace Mono.test_585
{
    interface IA
    {
    }

    interface IB
    {
    }

    class A
    {
    }

    class B
    {
    }

    class X : IA
    {
    }

    class Program
    {
        [Uno.Testing.Test] public static void test_585() { Main(); }
        public static void Main()
        {
            IA a = null;
            IB b = null;
            bool r = a == b;

            A aa = null;
            B bb = null;
            // Only this fails
            //r = aa == bb;

            X x = null;
            r = x == a;
            r = x == b;

            object o = null;
            r = o == x;
            r = o == a;
            r = o == aa;
        }
    }
}
