using System.Collections.Generic;

namespace Uno.Compiler.API.Backends
{
    public interface IJsBackend
    {
        bool Minify { get; }
        List<string> SourceFiles { get; }
    }
}