using System;

namespace Uno.Disasm
{
    [Flags]
    public enum VisibilityFlags
    {
        None,
        PublicOnly = 1 << 0,
        ProjectOnly = 1 << 1,
    }
}
