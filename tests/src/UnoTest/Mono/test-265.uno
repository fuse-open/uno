namespace Mono.test_265
{
    using Uno;
    
    internal class ClassFormatError
    {
        internal ClassFormatError(string msg, params object[] p)
        {
        }
    
        [Uno.Testing.Test] public static void test_265() { Main(); }
        public static void Main()
        { }
    }
    
    internal class UnsupportedClassVersionError : ClassFormatError
    {
        internal UnsupportedClassVersionError(string msg)
            : base(msg)
        {
        }
    }
}
