namespace Mono.gtest_301
{
    // Compiler options: -t:library
    
    using Uno;
    
    public static class Factory<BaseType> where BaseType : class
    {
        public static BaseType CreateInstance (params object[] args)
        {
            return null;
        }
    }
}
