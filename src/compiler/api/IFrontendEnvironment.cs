namespace Uno.Compiler.API
{
    public interface IFrontendEnvironment
    {
        bool CanCacheIL { get; }
        bool Parallel { get; }
        bool Test(Source src, string optionalCond);
    }
}