namespace Uno.Testing
{
    class AssertionFailedException : Uno.Exception
    {
        public readonly string FileName;
        public readonly int Line;
        public readonly string MemberName;
        public readonly object Expected;
        public readonly object Actual;

        public AssertionFailedException(string fileName, int line, string memberName, string expected, string actual)
        {
            this.FileName = fileName;
            this.Line = line;
            this.MemberName = memberName;
            this.Expected = expected;
            this.Actual = actual;
        }
    }
}
