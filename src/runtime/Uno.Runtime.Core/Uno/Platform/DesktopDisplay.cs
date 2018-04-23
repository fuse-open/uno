// This file was generated based on Library/Core/UnoCore/Source/Uno/Platform/Displays.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.Platform
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public class DesktopDisplay : Display
    {
        public DesktopDisplay()
        {
        }

        protected override float GetDensity()
        {
            global::Uno.Runtime.Implementation.PlatformWindowHandle wnd = global::Uno.Runtime.Implementation.PlatformWindowImpl.GetInstance();
            return global::Uno.Runtime.Implementation.PlatformWindowImpl.GetDensity(wnd);
        }
    }
}
