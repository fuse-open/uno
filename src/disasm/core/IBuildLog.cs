using System.IO;

namespace Uno.Disasm
{
    public interface IBuildLog
    {
        TextWriter Writer { get; }
        void Clear();
    }
}