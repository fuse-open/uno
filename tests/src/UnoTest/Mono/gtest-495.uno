namespace Mono.gtest_495
{
    class Repro
    {
        class Outer
        {
            public class Inner<T> where T : class
            {
                public static T[] Values;
            }
        }
        [Uno.Testing.Test] public static void gtest_495() { Main(); }
        public static void Main()
        {
            Outer.Inner<string>.Values = new string[0];
        }
    }
}
