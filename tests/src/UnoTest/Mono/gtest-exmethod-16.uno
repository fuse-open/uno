namespace Mono.gtest_exmethod_16
{
    using Uno;

    static class Rocks
    {
        public static bool Extension (this string self)
        {
            return true;
        }

        public static bool Extension (this D self)
        {
            return true;
        }
    }

    delegate string D ();

    class Program
    {
        event D e;

        public string this [int index] {
            get { return "HelloWorld"; }
        }

        public string Property {
            get { return "a"; }
        }

        [Uno.Testing.Test] public static void gtest_exmethod_16() { Main(new string[0]); }
        public static void Main(string[] args)
        {
            Program p = new Program ();
            p [0].Extension ();
            p.Property.Extension ();
            p.e.Extension ();
        }
    }
}
