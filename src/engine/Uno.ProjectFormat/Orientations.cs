using System;

namespace Uno.ProjectFormat
{
    [Flags]
    public enum Orientations
    {
        Portrait = 1 << 0,
        PortraitUpsideDown = 1 << 1,
        LandscapeLeft = 1 << 2,
        LandscapeRight = 1 << 3,
        Landscape = LandscapeLeft | LandscapeRight,
        Auto = Landscape | Portrait | PortraitUpsideDown
    }
}