namespace Uno.Compiler.API.Domain.Extensions
{
    public interface IDisambiguable
    {
        Source Source { get; }
        Disambiguation Disambiguation { get; }
    }
}