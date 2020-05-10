using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Uno.Build.Stuff
{
    static class LongPathDisk
    {
        public static void CreateDirectory(string pathName)
        {
            if (!WindowsInterop.CreateDirectory(pathName.ToLongPath(), IntPtr.Zero))
            {
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 0xB7 /* Already exists */)
                    return;

                var innerException = new Win32Exception(errorCode);
                throw new IOException("Failed to create directory '" + pathName + "'.\n" + innerException.Message, innerException);
            }
        }

        public static void WriteAllBytes(string fileName, byte[] data)
        {
            using (var file = WindowsInterop.CreateFile(
                fileName.ToLongPath(),
                dwDesiredAccess: WindowsInterop.AccessMask.Write,
                dwShareMode: 0,
                lpSecurityAttributes: IntPtr.Zero,
                dwCreationDisposition: WindowsInterop.CreateAttributes.CreateAlways,
                dwFlagsAndAttributes: FileAttributes.Normal,
                hTemplateFile: IntPtr.Zero))
            {
                if (file.IsInvalid)
                {
                    var innerException = new Win32Exception(Marshal.GetLastWin32Error());
                    throw new IOException("Failed to create or open file '" + fileName + "'.\n" + innerException.Message, innerException);
                }

                uint bytesWritten = 0;
                if (!WindowsInterop.WriteFile(file, data, (uint)data.Length, out bytesWritten, IntPtr.Zero))
                {
                    var innerException = new Win32Exception(Marshal.GetLastWin32Error());
                    throw new IOException("Failed to write to file '" + fileName + "'.\n" + innerException.Message, innerException);
                }
            }
        }

        public static void DeleteFile(string fileName)
        {
            if (!WindowsInterop.DeleteFile(fileName.ToLongPath()))
            {
                var innerException = new Win32Exception(Marshal.GetLastWin32Error());
                throw new IOException("Failed to delete file '" + fileName + "'.\n" + innerException.Message, innerException);
            }
        }

        public static bool DirectoryExists(string path)
        {
            var attrib = WindowsInterop.GetFileAttributes(path.ToLongPath());
            if (attrib == -1)
                return false;

            return ((FileAttributes)attrib).HasFlag(FileAttributes.Directory);
        }

        public static bool FileExists(string path)
        {
            var attrib = WindowsInterop.GetFileAttributes(path.ToLongPath());
            if (attrib == -1)
                return false;

            return ((FileAttributes)attrib).HasFlag(FileAttributes.Directory) == false;
        }

        public static void SetFileAttributes(string path, FileAttributes fileAttributes)
        {
            if (!WindowsInterop.SetFileAttributes(path.ToLongPath(), fileAttributes))
            {
                var innerException = new Win32Exception(Marshal.GetLastWin32Error());
                throw new IOException("Failed to set file attributes on '" + path + "'.\n" + innerException.Message, innerException);
            }
        }

        public static void DeleteDirectory(string fullPath, bool recursive)
        {
            DeleteDirectoryHelper(fullPath, recursive, true);
        }

        public static void DeleteDirectoryHelper(string fullPath, bool recursive, bool throwOnDirNotFound)
        {
            if (recursive)
            {
                WindowsInterop.WIN32_FIND_DATA data = new WindowsInterop.WIN32_FIND_DATA();
                using (WindowsInterop.SafeFindHandle hnd = WindowsInterop.FindFirstFile(@"\\?\" + fullPath + Path.DirectorySeparatorChar + "*", data))
                {
                    if (hnd.IsInvalid)
                    {
                        var innerException = new Win32Exception(Marshal.GetLastWin32Error());
                        throw new IOException("Failed to traverse directory '" + fullPath + "'.\n" + innerException.Message, innerException);
                    }

                    do
                    {
                        bool isDir = (0 != (data.dwFileAttributes & (int)FileAttributes.Directory));
                        if (isDir)
                        {
                            if (data.cFileName.Equals(".") || data.cFileName.Equals(".."))
                                continue;

                            bool shouldRecurse = (0 == (data.dwFileAttributes & (int)FileAttributes.ReparsePoint));
                            if (shouldRecurse)
                            {
                                var newFullPath = Path.Combine(fullPath, data.cFileName);
                                DeleteDirectoryHelper(newFullPath, recursive, false);
                            }
                            else
                            {
                                if (data.dwReserved0 == unchecked((int)0xA0000003) /*IO_REPARSE_TAG_MOUNT_POINT*/)
                                {
                                    var mountPoint = Path.Combine(fullPath, data.cFileName + Path.DirectorySeparatorChar);
                                    if (!WindowsInterop.DeleteVolumeMountPoint(mountPoint))
                                    {
                                        var errorCode = Marshal.GetLastWin32Error();
                                        if (errorCode != 0x3 /* ERROR_PATH_NOT_FOUND */)
                                        {
                                            var innerException = new Win32Exception(errorCode);
                                            throw new IOException("Failed to delete mount point '" + mountPoint + "'.\n" + innerException.Message, innerException);
                                        }
                                    }
                                }

                                var reparsePoint = Path.Combine(fullPath, data.cFileName);
                                if (!WindowsInterop.RemoveDirectory(reparsePoint.ToLongPath()))
                                {
                                    var errorCode = Marshal.GetLastWin32Error();
                                    if (errorCode != 0x3 /* ERROR_PATH_NOT_FOUND */)
                                    {
                                        var innerException = new Win32Exception(errorCode);
                                        throw new IOException("Failed to delete reparse point '" + reparsePoint + "'.\n" + innerException.Message, innerException);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DeleteFile(Path.Combine(fullPath, data.cFileName));
                        }
                    } while (WindowsInterop.FindNextFile(hnd, data));
                }
            }

            if (!WindowsInterop.RemoveDirectory(fullPath.ToLongPath()))
            {
                var errorCode = Marshal.GetLastWin32Error();
                if (!throwOnDirNotFound && (errorCode == 0x3/* ERROR_PATH_NOT_FOUND */ || errorCode == 0x2 /* ERROR_FILE_NOT_FOUND */))
                    return;

                var innerException = new Win32Exception(errorCode);
                throw new IOException("Failed to delete directory '" + fullPath + "'.\n" + innerException.Message, innerException);
            }
        }

        static string ToLongPath(this string path)
        {
            const int maxPathLength = 150; // MAX_PATH is set to 260, however eg. XP has 152 as real max length.
            return path.Length > maxPathLength ? @"\\?\" + path : Path.GetFullPath(path);
        }

        class WindowsInterop
        {
            const string Kernel32 = "kernel32.dll";
            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
            public extern static bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);

            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
            public static extern SafeFileHandle CreateFile(string lpFileName, AccessMask dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, CreateAttributes dwCreationDisposition, FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint written, IntPtr lpOverlapped);

            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
            public static extern bool DeleteFile(string lpFileName);

            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
            public static extern bool RemoveDirectory(string lpPathName);

            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
            public static extern int GetFileAttributes(string lpFileName);

            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
            public static extern bool SetFileAttributes(string lpFileName, FileAttributes dwFileAttributes);

            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
            public static extern SafeFindHandle FindFirstFile(string fileName, [In, Out] WIN32_FIND_DATA data);

            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
            public static extern bool FindNextFile(SafeFindHandle hndFindFile, [In, Out, MarshalAs(UnmanagedType.LPStruct)]WIN32_FIND_DATA lpFindFileData);

            [DllImport(Kernel32, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
            public static extern bool DeleteVolumeMountPoint(String mountPoint);

            [DllImport(Kernel32)]
            public static extern bool FindClose(IntPtr handle);

            [Flags]
            public enum AccessMask : uint
            {
                Read = 0x80000000,
                Write = 0x40000000,
                Execute = 0x20000000,
                All = 0x10000000,
            }

            public enum CreateAttributes
            {
                CreateNew = 1,
                CreateAlways = 2,
                OpenExisting = 3,
                OpenAlways = 4,
                TruncateExisting = 5
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            [BestFitMapping(false)]
            public class WIN32_FIND_DATA
            {
                public int dwFileAttributes = 0;

                public uint ftCreationTime_dwLowDateTime = 0;
                public uint ftCreationTime_dwHighDateTime = 0;

                public uint ftLastAccessTime_dwLowDateTime = 0;
                public uint ftLastAccessTime_dwHighDateTime = 0;

                public uint ftLastWriteTime_dwLowDateTime = 0;
                public uint ftLastWriteTime_dwHighDateTime = 0;
                public int nFileSizeHigh = 0;
                public int nFileSizeLow = 0;

                public int dwReserved0 = 0;
                public int dwReserved1 = 0;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName = null;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
                public string cAlternateFileName = null;
            }

            public sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
            {
                internal SafeFindHandle() : base(true) { }

                override protected bool ReleaseHandle()
                {
                    return FindClose(handle);
                }
            }
        }
    }
}
