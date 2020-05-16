namespace Uno.Compiler.API.Utilities
{
    public interface IKeywords
    {
        bool IsReserved(string identifier);
    }
}