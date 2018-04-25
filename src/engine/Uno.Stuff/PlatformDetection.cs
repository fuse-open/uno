using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Stuff
{
    public static class PlatformDetection
    {
        public static readonly bool IsWindows;
        public static readonly bool IsLinux;
        public static readonly bool IsMac;
        public static readonly bool IsArm;

        static PlatformDetection()
        {
            IsWindows = Path.DirectorySeparatorChar == '\\';

            if (IsWindows)
                return;

            try
            {
                _Utsname utsname;
                if (Mono_Posix_Syscall_uname(out utsname) == 0)
                {
                    try
                    {
                        var sysname = Marshal.PtrToStringAnsi(utsname.sysname);
                        var machine = Marshal.PtrToStringAnsi(utsname.machine);
                        IsLinux = sysname == "Linux";
                        IsMac = sysname == "Darwin";
                        IsArm = machine.StartsWith("arm");
                    }
                    finally
                    {
                        free(utsname._buf_);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warning("uname() failed: " + e.Message);
            }
        }

        struct _Utsname
        {
            public IntPtr sysname;
            public IntPtr nodename;
            public IntPtr release;
            public IntPtr version;
            public IntPtr machine;
            public IntPtr domainname;
            public IntPtr _buf_;
        }

        [DllImport ("MonoPosixHelper", SetLastError=true)]
        static extern int Mono_Posix_Syscall_uname(out _Utsname buf);

        [DllImport ("msvcrt", CallingConvention=CallingConvention.Cdecl)]
        static extern void free(IntPtr ptr);
    }
}
