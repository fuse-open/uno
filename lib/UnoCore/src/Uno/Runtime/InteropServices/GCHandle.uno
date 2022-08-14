using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Runtime.InteropServices
{
    [extern(DOTNET) DotNetType("System.Runtime.InteropServices.GCHandle")]
    public struct GCHandle
    {
        public object Target { get; private set; }

        extern(!DOTNET) GCHandle(object target)
        {
            Target = target;
        }

        public static GCHandle Alloc(object target)
        {
            return Alloc(target, GCHandleType.Normal);
        }

        public static GCHandle Alloc(object target, GCHandleType type)
        {
            if defined(CPLUSPLUS)
            {
                // type doesn't matter in C++ since we never relocate objects
                @{
                    ::uRetain($0);
                @}
                return new GCHandle(target);
            }
            else
                build_error;
        }

        public void Free()
        {
            if defined(CPLUSPLUS)
            {
                extern(Target)
                @{
                    ::uRelease($0);
                @}
            }
            else
                build_error;
        }

        public static GCHandle FromIntPtr(IntPtr ptr)
        {
            if defined(CPLUSPLUS)
            {
                var obj = extern<object> "(@{object})$0";
                return new GCHandle(obj);
            }
            else
                build_error;
        }

        public static IntPtr ToIntPtr(GCHandle handle)
        {
            if defined(CPLUSPLUS)
            {
                return extern<IntPtr>(handle.Target) "$0";
            }
            else
                build_error;
        }

        public static explicit operator GCHandle(IntPtr ptr) { return FromIntPtr(ptr); }
        public static explicit operator IntPtr(GCHandle handle) { return ToIntPtr(handle); }

        public IntPtr AddrOfPinnedObject()
        {
            var obj = Target;
            var arr = obj as Array;
            return arr != null
                ? extern<IntPtr>(arr) "$0->Ptr()"
                : extern<IntPtr>(obj) "$0";
        }
    }
}
