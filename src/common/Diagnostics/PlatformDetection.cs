using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Uno.Logging;

#pragma warning disable 649
#pragma warning disable 169

namespace Uno.Diagnostics
{
    public static class PlatformDetection
    {
        public static readonly bool IsWindows;
        public static readonly bool IsLinux;
        public static readonly bool IsMac;
        public static readonly bool IsArm;
        public static readonly bool Is64Bit;

        public static string HomeDirectory
        {
            get
            {
                var home = Environment.GetEnvironmentVariable("HOME");
                if (!string.IsNullOrEmpty(home))
                    return home;

                if (IsWindows)
                {
                    var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
                    if (!string.IsNullOrEmpty(userProfile))
                        return userProfile;

                    var homePath = Environment.GetEnvironmentVariable("HOMEDRIVE") +
                                   Environment.GetEnvironmentVariable("HOMEPATH");
                    if (!string.IsNullOrEmpty(homePath))
                        return homePath;
                }

                throw new NotSupportedException("Your home directory was not found");
            }
        }

        public static string SystemString
        {
            get
            {
                if (IsWindows)
                    return WindowsVersion + " " + (
                            Is64Bit
                                ? "x64"
                                : "x86"
                        );
                if (IsMac)
                    // macOS version should be roughly 10.(KERNEL_MAJOR - 4)
                    return "macOS 10." + (Environment.OSVersion.Version.Major - 4) + " " + (
                            Is64Bit
                                ? "x86_64"
                                : "i386"
                        );
                if (IsLinux)
                    return "Linux " + Environment.OSVersion.Version.ToString(2) + " " + (
                            IsArm
                                ? "ARM" + (Is64Bit ? "64" : null) :
                            Is64Bit
                                ? "x86_64"
                                : "x86_32"
                        );
                return Environment.OSVersion.VersionString + " (unsupported)";
            }
        }

        static string WindowsVersion
        {
            get
            {
                try
                {
                    var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                    var productName = (string)reg.GetValue("ProductName");
                    var productParts = productName.Split(' ');
                    return productParts[1] == "Server"
                        ? productParts[0] + " " + productParts[1] + " " + productParts[2]
                        : productParts[0] + " " + productParts[1];
                }
                catch
                {
                    return "Windows (unsupported)";
                }
            }
        }

        static PlatformDetection()
        {
            IsWindows = Path.DirectorySeparatorChar == '\\';
            Is64Bit = IntPtr.Size == 8;

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
                        IsArm = machine.StartsWith("arm", StringComparison.InvariantCulture);
                    }
                    finally
                    {
                        free(utsname._buf_);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Default.Warning("uname() failed: " + e.Message);
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

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern int Mono_Posix_Syscall_uname(out _Utsname buf);

        [DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl)]
        static extern void free(IntPtr ptr);
    }
}
