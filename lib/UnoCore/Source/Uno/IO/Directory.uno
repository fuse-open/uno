using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    public enum UserDirectory
    {
        Cache,
        Config,
        Data,
        Desktop,
        Downloads,
        Templates,
        Public,
        Documents,
        Music,
        Pictures,
        Videos,
    }

    [extern(DOTNET) DotNetType("System.IO.Directory")]
    [extern(CPLUSPLUS && UNIX) Require("Source.Include", "errno.h")]
    [extern(CPLUSPLUS && UNIX) Require("Source.Include", "fcntl.h")]
    [extern(CPLUSPLUS && UNIX) Require("Source.Include", "sys/stat.h")]
    [extern(CPLUSPLUS && UNIX) Require("Source.Include", "sys/types.h")]
    [extern(CPLUSPLUS && WIN32) Require("Source.Include", "Uno/WinAPIHelper.h")]
    [extern(CPLUSPLUS && APPLE) Require("Source.FileExtension", "mm")]
    public static class Directory
    {
        [DotNetOverride]
        public static string GetUserDirectory(UserDirectory dir)
        {
            if defined(DOTNET)
            {
                switch (dir)
                {
                    case UserDirectory.Cache:
                        return Path.GetTempPath();
                    case UserDirectory.Config:
                        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                    case UserDirectory.Data:
                        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                    case UserDirectory.Desktop:
                        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
                    case UserDirectory.Templates:
                        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Templates);
                    case UserDirectory.Documents:
                        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                    case UserDirectory.Music:
                        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic);
                    case UserDirectory.Pictures:
                        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
                }
            }
            else if defined(WIN32)
            {
                switch (dir)
                {
                    case UserDirectory.Cache:
                        return Path.GetTempPath();
                    case UserDirectory.Config:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_RoamingAppData");
                    case UserDirectory.Data:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_LocalAppData");
                    case UserDirectory.Desktop:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_Desktop");
                    case UserDirectory.Downloads:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_Downloads");
                    case UserDirectory.Templates:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_Templates");
                    case UserDirectory.Public:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_Public");
                    case UserDirectory.Documents:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_Documents");
                    case UserDirectory.Music:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_Music");
                    case UserDirectory.Pictures:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_Pictures");
                    case UserDirectory.Videos:
                        return GetUserDirectoryWin32(extern<IntPtr> "(void*) &FOLDERID_Videos");
                }
            }
            else if defined(APPLE)
            {
                switch (dir)
                {
                    case UserDirectory.Cache:
                        return GetUserDirectoryApple(extern<int> "NSCachesDirectory");
                    case UserDirectory.Config:
                        return GetUserDirectoryApple(extern<int> "NSLibraryDirectory");
                    case UserDirectory.Data:
                        return GetUserDirectoryApple(extern<int> "NSLibraryDirectory");
                    case UserDirectory.Desktop:
                        return GetUserDirectoryApple(extern<int> "NSDesktopDirectory");
                    case UserDirectory.Downloads:
                        return GetUserDirectoryApple(extern<int> "NSDownloadsDirectory");
                    case UserDirectory.Templates:
                        return GetUserDirectoryApple(extern<int> "NSUserDirectory") + "/Templates";
                    case UserDirectory.Public:
                        return GetUserDirectoryApple(extern<int> "NSSharedPublicDirectory");
                    case UserDirectory.Documents:
                        return GetUserDirectoryApple(extern<int> "NSDocumentDirectory");
                    case UserDirectory.Music:
                        return GetUserDirectoryApple(extern<int> "NSMusicDirectory");
                    case UserDirectory.Pictures:
                        return GetUserDirectoryApple(extern<int> "NSPicturesDirectory");
                    case UserDirectory.Videos:
                        return GetUserDirectoryApple(extern<int> "NSMoviesDirectory");
                }
            }
            else if defined(ANDROID)
            {
                switch (dir)
                {
                    case UserDirectory.Config:
                        return GetUserConfigDirectory();
                    case UserDirectory.Data:
                        return GetUserDataDirectory();
                    default:
                        return "/sdcard";
                }
            }
            else if defined(LINUX)
            {
                switch (dir)
                {
                    case UserDirectory.Cache:
                        return GetUserDirectoryLinux("XDG_CACHE_HOME", ".cache");
                    case UserDirectory.Config:
                        return GetUserDirectoryLinux("XDG_CONFIG_HOME", ".config");
                    case UserDirectory.Data:
                        return GetUserDirectoryLinux("XDG_DATA_HOME", ".local/share");
                    case UserDirectory.Desktop:
                        return GetUserDirectoryLinux("XDG_DESKTOP_DIR", "Desktop");
                    case UserDirectory.Downloads:
                        return GetUserDirectoryLinux("XDG_DOWNLOAD_DIR", "Downloads");
                    case UserDirectory.Templates:
                        return GetUserDirectoryLinux("XDG_TEMPLATES_DIR", "Templates");
                    case UserDirectory.Public:
                        return GetUserDirectoryLinux("XDG_PUBLICSHARE_DIR", "Public");
                    case UserDirectory.Documents:
                        return GetUserDirectoryLinux("XDG_DOCUMENTS_DIR", "Documents");
                    case UserDirectory.Music:
                        return GetUserDirectoryLinux("XDG_MUSIC_DIR", "Music");
                    case UserDirectory.Pictures:
                        return GetUserDirectoryLinux("XDG_PICTURES_DIR", "Pictures");
                    case UserDirectory.Videos:
                        return GetUserDirectoryLinux("XDG_VIDEOS_DIR", "Videos");
                }
            }

            throw new ArgumentOutOfRangeException(nameof(dir));
        }

        [Foreign(Language.Java)]
        extern(ANDROID)
        static string GetUserConfigDirectory()
        @{
            android.content.Context context = com.fuse.Activity.getRootActivity();
            java.io.File dir = context.getExternalFilesDir(null);
            return dir.getAbsolutePath();
        @}

        [Foreign(Language.Java)]
        extern(ANDROID)
        static string GetUserDataDirectory()
        @{
            android.content.Context context = com.fuse.Activity.getRootActivity();
            java.io.File dir = context.getFilesDir();
            return dir.getAbsolutePath();
        @}

        [Foreign(Language.ObjC)]
        extern(APPLE)
        static string GetUserDirectoryApple(int directory)
        @{
            return NSSearchPathForDirectoriesInDomains((NSSearchPathDirectory) directory, NSUserDomainMask, YES)[0];
        @}

        extern(LINUX)
        static string GetUserDirectoryLinux(string env, string name)
        {
            @{
                const char* dir = getenv(uCString($0).Ptr);
                if (dir && strlen(dir))
                    return uString::Utf8(dir);

                const char* homedir = getenv("HOME");
                if (!homedir || !strlen(homedir))
                    U_THROW_IOE("Invalid home directory");
            @}
            return extern<string> "uString::Utf8(homedir)" + "/" + name;
        }

        extern(WIN32)
        static string GetUserDirectoryWin32(IntPtr rfid)
        @{
            PWSTR pszPath;
            if (SHGetKnownFolderPath(*(const KNOWNFOLDERID*) $0, KF_FLAG_NO_ALIAS, 0, &pszPath) != S_OK)
                U_THROW_IOE("SHGetKnownFolderPath() failed");

            uString* retval = uString::Utf16((const char16_t*) pszPath);
            CoTaskMemFree(pszPath);
            return retval;
        @}

        [extern(DOTNET) DotNetOverride]
        [extern(APPLE) Require("Source.Import", "Foundation/Foundation.h")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{Path:Include}")]
        internal static string GetBaseDirectory()
        {
            if defined(WIN32)
            @{
                WCHAR buf[4096];
                DWORD result = GetModuleFileNameW(GetModuleHandle(0), buf, sizeof(buf));

                if (result < sizeof(buf))
                    return @{Path.GetDirectoryName(string):Call(uString::Utf16((const char16_t*) buf, result))};

                U_THROW_IOE("GetModuleFileNameW() buffer overrun");
            @}
            else if defined(APPLE)
            @{
                NSArray* arguments = [[NSProcessInfo processInfo] arguments];
                NSString* exe = [arguments objectAtIndex:0];
                return @{Path.GetDirectoryName(string):Call(uString::Utf8([exe UTF8String]))};
            @}
            else if defined(CPLUSPLUS)
            @{
                char path[4096];
                char dest[4096];
                pid_t pid = getpid();
                snprintf(path, sizeof(path), "/proc/%d/exe", pid);

                if (readlink(path, dest, sizeof(dest)) != -1)
                    return @{Path.GetDirectoryName(string):Call(uString::Utf8(dest))};

                U_THROW_IOE("GetBaseDirectory() failed");
            @}
            else
                throw new NotImplementedException();
        }

        public static string GetCurrentDirectory()
        {
            if defined(WIN32)
            @{
                WCHAR buf[4096];
                GetCurrentDirectoryW(sizeof(buf), buf);
                return uString::Utf16((const char16_t*) buf);
            @}
            else if defined(CPLUSPLUS)
            @{
                char buf[4096];
                if (getcwd(buf, sizeof(buf)) != buf)
                    U_THROW_IOE("getcwd() failed");

                return uString::Utf8(buf);
            @}
            else
                throw new NotImplementedException();
        }

        public static void SetCurrentDirectory(string dirName)
        {
            if (dirName == null)
                throw new ArgumentNullException(nameof(dirName));

            if defined(WIN32)
            @{
                SetCurrentDirectoryW((LPCWSTR) $0->Ptr());
            @}
            else if defined(CPLUSPLUS)
            @{
                int res = chdir(uCString($0).Ptr);
            @}
            else
                throw new NotImplementedException();
        }

        [DotNetOverride]
        public static void CreateDirectory(string dirName)
        {
            if defined(DOTNET)
                System.IO.Directory.CreateDirectory(dirName);
            else
            {
                if (dirName == null)
                    throw new ArgumentNullException(nameof(dirName));

                if (dirName.Length == 0 ||
                        defined(WIN32) &&  // Skip Windows drive letters
                            dirName.Length == 2 && dirName[1] == ':' ||
                        Exists(dirName))
                    return;

                // Create parent directory (recursive)
                CreateDirectory(Path.GetDirectoryName(dirName));

                if defined(WIN32)
                @{
                    if (CreateDirectoryW((LPCWSTR) $0->Ptr(), 0) ||
                            GetLastError() == ERROR_ALREADY_EXISTS)
                        return;
                @}
                else if defined(CPLUSPLUS)
                @{
                    if (mkdir(uCString($0).Ptr, S_IRWXU | S_IRWXG | S_IROTH | S_IXOTH) == 0 ||
                            errno == EEXIST)
                        return;
                @}
                else
                    throw new NotImplementedException();

                throw new IOException("Unable to create directory '" + dirName + "'");
            }
        }

        public static void Delete(string dirName, bool recursive)
        {
            if (dirName == null)
                throw new ArgumentNullException(nameof(dirName));

            if (recursive)
            {
                foreach (var e in EnumerateDirectories(dirName))
                    Delete(e, true);
                foreach (var e in EnumerateFiles(dirName))
                    File.Delete(e);
            }

            if defined(WIN32)
            {
                if (!extern<bool> "RemoveDirectoryW((LPCWSTR) $0->Ptr())")
                    throw new IOException("Unable to delete directory '" + dirName + "': " + WinAPI.GetLastErrorString());
            }
            else if defined(CPLUSPLUS)
            {
                extern "uCString cstr($0)";
                if (extern<int> "rmdir(cstr.Ptr)" != 0)
                    throw new IOException("Unable to delete directory '" + dirName + "'");
            }
            else
                throw new NotImplementedException();
        }

        public static void Move(string oldName, string newName)
        {
            if (oldName == null)
                throw new ArgumentNullException(nameof(oldName));
            if (newName == null)
                throw new ArgumentNullException(nameof(newName));

            if defined(WIN32)
            {
                if (!extern<bool> "MoveFileW((LPCWSTR) $0->Ptr(), (LPCWSTR) $1->Ptr())")
                    throw new IOException("Unable to move directory '" + oldName + "' to '" + newName + "': " + WinAPI.GetLastErrorString());
            }
            else if defined(CPLUSPLUS)
            {
                extern "uCString oldNameU8($0)";
                extern "uCString newNameU8($1)";
                if (extern<int> "rename(oldNameU8.Ptr, newNameU8.Ptr)" != 0)
                    throw new IOException("Unable to move directory '" + oldName + "' to '" + newName + "'");
            }
            else
                throw new NotImplementedException();
        }

        public static bool Exists(string dirName)
        {
            if (dirName == null)
                throw new ArgumentNullException(nameof(dirName));

            if defined(WIN32)
            @{
                WIN32_FILE_ATTRIBUTE_DATA data;
                return GetFileAttributesEx((LPCWSTR) $0->Ptr(), GetFileExInfoStandard, &data) &&
                    (data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY);
            @}
            else if defined(CPLUSPLUS)
            @{
                struct stat attributes;
                return stat(uCString($0).Ptr, &attributes) != -1 &&
                    S_ISDIR(attributes.st_mode);
            @}
            else
                throw new NotImplementedException();
        }

        public static IEnumerable<string> EnumerateDirectories(string dirName)
        {
            return new Enumerable(dirName, EnumeratorMode.Directories);
        }

        public static IEnumerable<string> EnumerateFiles(string dirName)
        {
            return new Enumerable(dirName, EnumeratorMode.Files);
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string dirName)
        {
            return new Enumerable(dirName, EnumeratorMode.AllEntries);
        }

        class Enumerable : IEnumerable<string>
        {
            readonly string _dirName;
            readonly EnumeratorMode _mode;

            public Enumerable(string dirName, EnumeratorMode mode)
            {
                _dirName = dirName;
                _mode = mode;
            }

            public IEnumerator<string> GetEnumerator()
            {
                return new Enumerator(_dirName, _mode);
            }
        }

        [extern(CPLUSPLUS && UNIX) Require("Source.Include", "dirent.h")]
        class Enumerator : IEnumerator<string>
        {
            extern(WIN32) readonly string _filter;
            extern(WIN32) IntPtr _findData;
            readonly EnumeratorMode _mode;
            readonly string _prefix;
            string _current;
            IntPtr _handle;

            public string Current { get { return _current; } }

            public Enumerator(string dirName, EnumeratorMode mode)
            {
                if (dirName == null)
                    throw new ArgumentNullException(nameof(dirName));

                dirName = dirName.TrimEnd(Path.DirectorySeparatorChar);

                if (!Exists(dirName))
                    throw new FileNotFoundException("Directory not found: " + dirName, dirName);

                _mode = mode;
                _prefix = dirName != "."
                    ? dirName + Path.DirectorySeparatorChar
                    : "";

                if defined(WIN32)
                {
                    _filter = _prefix + "*";
                    _findData = extern<IntPtr> "new WIN32_FIND_DATA";
                }
            }

            public void Dispose()
            {
                Reset();

                if defined(WIN32)
                @{
                    delete (WIN32_FIND_DATA*) @{$$._findData};
                    @{$$._findData} = nullptr;
                @}
            }

            public void Reset()
            {
                _current = null;

                if defined(WIN32)
                @{
                    FindClose((HANDLE) @{$$._handle});
                    @{$$._handle} = nullptr;
                @}
                else if defined(UNIX)
                @{
                    closedir((DIR*) @{$$._handle});
                    @{$$._handle} = nullptr;
                @}
            }

            public bool MoveNext()
            {
                if defined(WIN32)
                {
                    @{
                        WIN32_FIND_DATA* findData = (WIN32_FIND_DATA*) @{$$._findData};
                        HANDLE handle = (HANDLE) @{$$._handle};
                    @}

                    for (;;)
                    {
                        @{
                            if (!@{$$._handle})
                            {
                                handle = FindFirstFileW((LPCWSTR) @{$$._filter}->Ptr(), findData);
                                if (handle == INVALID_HANDLE_VALUE)
                                    return false;

                                @{$$._handle} = handle;
                            }
                            else if (!FindNextFile(handle, findData))
                                return false;

                            if (!wcscmp(L".", findData->cFileName) || !wcscmp(L"..", findData->cFileName))
                                continue;

                            switch (@{$$._mode})
                            {
                                case @{EnumeratorMode.Directories}:
                                    if ((findData->dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) == 0)
                                        continue;
                                    break;
                                case @{EnumeratorMode.Files}:
                                    if ((findData->dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0)
                                        continue;
                                    break;
                            }
                        @}

                        _current = _prefix + extern<string> "uString::Utf16((const char16_t*) findData->cFileName)";
                        return true;
                    }
                }
                else if defined(UNIX)
                {
                    if (_handle == IntPtr.Zero)
                        _handle = extern<IntPtr> "opendir(uCString(@{$$._prefix}).Ptr)";

                    extern "struct dirent *ep";
                    while (extern<bool> "(ep = readdir((DIR*) @{$$._handle}))")
                    {
                        @{
                            if (!strcmp(".", ep->d_name) || !strcmp("..", ep->d_name))
                                continue;
                        @}

                        _current = _prefix + extern<string> "uString::Utf8(ep->d_name)";

                        switch (_mode)
                        {
                            case EnumeratorMode.Directories:
                                if (!Exists(_current))
                                    continue;
                                break;
                            case EnumeratorMode.Files:
                                if (!File.Exists(_current))
                                    continue;
                                break;
                        }

                        return true;
                    }
                }

                return false;
            }
        }

        enum EnumeratorMode
        {
            AllEntries,
            Directories,
            Files
        }
    }
}
