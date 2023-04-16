using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32;
using Uno.CLI;

namespace Uno.Diagnostics
{
    public static class PlatformDetection
    {
        public static string HomeDirectory
        {
            get
            {
                var home = Environment.GetEnvironmentVariable("HOME");
                if (!string.IsNullOrEmpty(home))
                    return home;

                if (OperatingSystem.IsWindows())
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
                var arch = RuntimeInformation.OSArchitecture;

                if (OperatingSystem.IsWindows())
                    return WindowsVersion + " " + (
                            arch == Architecture.Arm
                                ? "arm32" :
                            arch == Architecture.Arm64
                                ? "arm64" :
                            IntPtr.Size == 8
                                ? "x64"
                                : "x86"
                        );
                if (OperatingSystem.IsMacOS())
                    return "macOS " + MacVersion + " " + (
                            arch == Architecture.Arm
                                ? "arm32" :
                            arch == Architecture.Arm64
                                ? "arm64" :
                            IntPtr.Size == 8
                                ? "x86_64"
                                : "i386"
                        );
                if (OperatingSystem.IsLinux())
                    return "Linux " + Environment.OSVersion.Version.ToString(2) + " " + (
                            arch == Architecture.Arm
                                ? "arm32" :
                            arch == Architecture.Arm64
                                ? "arm64" :
                            IntPtr.Size == 8
                                ? "x86_64"
                                : "x86_32"
                        );
                return Environment.OSVersion.VersionString + " (unsupported)";
            }
        }

        [SupportedOSPlatform("macOS")]
        static string MacVersion
        {
            get
            {
                try
                {
                    return Shell.Default.GetOutput("sw_vers").Grep("ProductVersion").First().Trim().Split(' ', '\t').Last();
                }
                catch
                {
                    return "10." + (Environment.OSVersion.Version.Major - 4);
                }
            }
        }

        [SupportedOSPlatform("windows")]
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
    }
}
