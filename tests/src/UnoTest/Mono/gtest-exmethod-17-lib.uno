namespace Mono.gtest_exmethod_17
{
    // Compiler options: -t:library
    
    using Uno;
    
    namespace Testy
    {
        public static class TestExtensions
        {
            public static string MyFormat (this Object junk,
                              string fmt, params object [] args)
            {
                return String.Format (fmt, args);
            }
        }
    }
}
