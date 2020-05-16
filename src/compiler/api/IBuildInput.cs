using System.Collections.Generic;

namespace Uno.Compiler.API
{
    public interface IBuildInput
    {
        IReadOnlyList<SourcePackage> Packages { get; }
        SourcePackage Package { get; }
    }
}