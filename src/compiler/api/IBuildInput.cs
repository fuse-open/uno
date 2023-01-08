using System.Collections.Generic;

namespace Uno.Compiler.API
{
    public interface IBuildInput
    {
        IReadOnlyList<SourceBundle> Bundles { get; }
        SourceBundle Bundle { get; }
    }
}