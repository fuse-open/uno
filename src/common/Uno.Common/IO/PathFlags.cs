using System;

namespace Uno.IO
{
    [Flags]
    public enum PathFlags
    {
        AllowAbsolutePath = 1 << 0,
        AllowNonExistingPath = 1 << 1,
        WarnIfNonExistingPath = 1 << 2,
        IsDirectory = 1 << 3,
    }
}