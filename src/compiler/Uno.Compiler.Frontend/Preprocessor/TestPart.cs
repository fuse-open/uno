namespace Uno.Compiler.Frontend.Preprocessor
{
    struct TestPart
    {
        public TestType Type;
        public bool? Condition;
        public int LineStart, LineEnd;

        public TestPart(TestType type, bool? cond, int start, int end)
        {
            Type = type;
            Condition = cond;
            LineStart = start;
            LineEnd = end;
        }
    }
}