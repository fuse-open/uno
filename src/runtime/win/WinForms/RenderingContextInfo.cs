using System;

namespace Uno.Support.WinForms
{
    public class RenderingContextInfo
    {
        public readonly IntPtr DeviceHandle;
        public readonly IntPtr ContextHandle;
        public RenderingContextInfo(IntPtr deviceHandle, IntPtr contextHandle)
        {
            DeviceHandle = deviceHandle;
            ContextHandle = contextHandle;
        }
    }
}