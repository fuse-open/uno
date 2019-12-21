// This file was generated based on lib/UnoCore/Source/Uno/Platform/Displays.uno.
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
            return WindowBackend.Instance.GetDensity();
        }
    }
}
