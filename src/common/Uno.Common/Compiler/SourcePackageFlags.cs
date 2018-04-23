using System;

namespace Uno.Compiler
{
    [Flags]
    public enum SourcePackageFlags
    {
        Startup = 1 << 0,
        Cached = 1 << 1,
        Transitive = 1 << 2,
        Project = 1 << 3,
        Verified = 1 << 4,
        CanLink = 1 << 5,
        FlatReferences = 1 << 6
    }
}