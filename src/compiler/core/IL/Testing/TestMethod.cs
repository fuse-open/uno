namespace Uno.Compiler.Core.IL.Testing
{
    public struct TestMethod
    {
        public readonly string Name;
        public readonly bool Ignored;
        public readonly string IgnoreReason;

        public TestMethod(string name, bool ignored, string ignoreReason)
        {
            Name = name;
            Ignored = ignored;
            IgnoreReason = ignoreReason;
        }
    }
}
