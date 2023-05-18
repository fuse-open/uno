using System;

namespace Uno.Compiler.API
{
    public interface ITranspiler : IDisposable
    {
        bool TryTranspile(string filename, string code, out string output);
    }
}
