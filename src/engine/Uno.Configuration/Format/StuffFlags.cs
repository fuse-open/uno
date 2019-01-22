using System;

namespace Uno.Configuration.Format
{
    [Flags]
    public enum StuffFlags
    {
        None = 0,
        AcceptAll = 1 << 0,
        Force = 1 << 1,
    }
}