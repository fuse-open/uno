namespace Mono.gtest_partial_03
{
    using Uno.Collections;

    class Variable
    {
    }

    internal partial class Test<T>
    {
    }

    internal partial class Test<T> where T : IList<Variable>
    {
        public Test (T t)
        {
            var val = t.Count;
        }
    }

    internal partial class Test<T>
    {
    }

    class CC
    {
        [Uno.Testing.Test] public static void gtest_partial_03() { Main(); }
        public static void Main()
        {
            new Test<List<Variable>> (new List<Variable> ());
        }
    }
}
