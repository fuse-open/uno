using System.Collections.Specialized;

namespace Uno.TestRunner.BasicTypes
{
    public struct Assertion
    {
        public readonly string Filename;
        public readonly int Line;
        public readonly string Membername;
        public readonly string Expected;
        public readonly string Actual;
        public readonly string Expression;

        public Assertion(string filename, int line, string membername, string expected, string actual, string expression)
        {
            Filename = filename;
            Line = line;
            Expected = expected;
            Actual = actual;
            Expression = expression;
            Membername = membername;
        }

        public override string ToString()
        {
            return base.ToString() +
                ": In " + Filename +
                ", line " + Line +
                ". Expected \"" + Expected +
                "\", but got \"" + Actual +
                "\". Expression: \"" + Expression + "\"";
        }

        public static Assertion From(NameValueCollection collection)
        {
            return new Assertion(
                collection["filename"],
                collection["line"] != null ? int.Parse(collection["line"]) : 0,
                collection["membername"],
                collection["expected"],
                collection["actual"],
                collection["expression"]
                );
        }
    }
}
