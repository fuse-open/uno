using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Text;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.File")]
    [extern(CPLUSPLUS && UNIX) Require("Source.Include", "fcntl.h")]
    [extern(CPLUSPLUS && UNIX) Require("Source.Include", "unistd.h")]
    [extern(CPLUSPLUS && UNIX) Require("Source.Include", "sys/stat.h")]
    [extern(CPLUSPLUS && UNIX) Require("Source.Include", "sys/types.h")]
    [extern(CPLUSPLUS && WIN32) Require("Source.Include", "Uno/WinAPIHelper.h")]
    public static class File
    {
        public static FileStream Open(string filename, FileMode filemode)
        {
            return new FileStream(filename, filemode);
        }

        public static FileStream OpenRead(string filename)
        {
            if defined(CPLUSPLUS)
                return new FileStream(FILEPtr.OpenOrThrow(filename, "rb"), true, false);
            else
                throw new NotImplementedException();
        }

        public static FileStream OpenWrite(string filename)
        {
            if defined(CPLUSPLUS)
                return new FileStream(FILEPtr.OpenOrThrow(filename, "wb"), false, true);
            else
                throw new NotImplementedException();
        }

        public static void Delete(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            if defined(WIN32)
            {
                if (!extern<bool> "DeleteFileW((LPCWSTR) $0->Ptr())")
                    throw new IOException("Unable to delete file '" + filename + "': " + WinAPI.GetLastErrorString());
            }
            else if defined(CPLUSPLUS)
            {
                extern "uCString cstr($0)";
                if (extern<int> "unlink(cstr.Ptr)" != 0)
                    throw new IOException("Unable to delete file '" + filename + "'");
            }
            else
                throw new NotImplementedException();
        }

        public static void Copy(string sourceFile, string destinationFile)
        {
            Copy(sourceFile, destinationFile, false);
        }

        public static void Copy(string sourceFile, string destinationFile, bool overwrite)
        {
            if (sourceFile == null)
                throw new ArgumentNullException(nameof(sourceFile));
            if (destinationFile == null)
                throw new ArgumentNullException(nameof(destinationFile));

            if defined(WIN32)
            {
                if (!extern<bool> "CopyFileW((LPCWSTR) $0->Ptr(), (LPCWSTR) $1->Ptr(), !$2)")
                    throw new IOException("Unable to copy file '" + sourceFile + "' to '" + destinationFile + "': " + WinAPI.GetLastErrorString());
            }
            else if defined(CPLUSPLUS)
            {
                extern int source;
                extern int destination;

                @{
                    uCString sourceFileU8($0);
                    source = open(sourceFileU8.Ptr, O_RDONLY);
                @}

                if (source == -1)
                    throw new IOException("Unable to copy from file '" + sourceFile + "'");

                @{
                    uCString destinationFileU8($1);
                    int createFlags = O_WRONLY | O_CREAT;
                    
                    if (!$2)
                        createFlags |= O_EXCL;

                    struct stat stat_buf;
                    fstat(source, &stat_buf);
                    destination = open(destinationFileU8.Ptr, createFlags, stat_buf.st_mode);
                @}

                if (destination == -1)
                {
                    extern "close(source)";
                    throw new IOException("Unable to copy to file '" + destinationFile + "'");
                }

                @{
                    char buf[BUFSIZ];
                    size_t size = 0;
                    ssize_t res;

                    while ((size = read(source, buf, BUFSIZ)) > 0)
                        res = write(destination, buf, size);

                    close(source);
                    close(destination);
                @}
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
                    throw new IOException("Unable to move file '" + oldName + "' to '" + newName + "': " + WinAPI.GetLastErrorString());
            }
            else if defined(CPLUSPLUS)
            {
                extern "uCString oldNameU8($0)";
                extern "uCString newNameU8($1)";
                if (extern<int> "rename(oldNameU8.Ptr, newNameU8.Ptr)" != 0)
                    throw new IOException("Unable to move file '" + oldName + "' to '" + newName + "'");
            }
            else
                throw new NotImplementedException();
        }

        public static bool Exists(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            if defined(WIN32)
            @{
                WIN32_FILE_ATTRIBUTE_DATA data;
                return GetFileAttributesEx((LPCWSTR) $0->Ptr(), GetFileExInfoStandard, &data) &&
                    !(data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY);
            @}
            else if defined(CPLUSPLUS)
            @{
                struct stat attributes;
                return stat(uCString($0).Ptr, &attributes) != -1 &&
                    !S_ISDIR(attributes.st_mode);
            @}
            else
                throw new NotImplementedException();
        }

        public static void AppendAllLines(string filename, string[] contents)
        {
            var sb = new StringBuilder();
            foreach (var content in contents)
                sb.AppendLine(content);

            AppendAllText(filename, sb.ToString());
        }

        public static void AppendAllText(string filename, string contents)
        {
            using (var w = new StreamWriter(Open(filename, FileMode.Append)))
                w.Write(contents);
        }

        public static string ReadAllText(string filename)
        {
            using (var r = new StreamReader(Open(filename, FileMode.Open)))
                return r.ReadToEnd();
        }

        public static string[] ReadAllLines(string filename)
        {
            var lines = new List<string>();
            using (var r = new StreamReader(Open(filename, FileMode.Open)))
                while (!r.EndOfStream)
                    lines.Add(r.ReadLine());
            return lines.ToArray();
        }

        public static byte[] ReadAllBytes(string filename)
        {
            using (var f = Open(filename, FileMode.Open))
            {
                var result = new byte[(int) f.Length];
                f.Read(result, 0, result.Length);
                return result;
            }
        }

        public static void WriteAllText(string filename, string text)
        {
            using (var w = new StreamWriter(Open(filename, FileMode.Create)))
                w.Write(text);
        }

        public static void WriteAllBytes(string filename, byte[] bytes)
        {
            using (var f = Open(filename, FileMode.Create))
                f.Write(bytes, 0, bytes.Length);
        }

        public static void WriteAllLines(string filename, string[] lines)
        {
            using (var w = new StreamWriter(Open(filename, FileMode.Create)))
                foreach (var line in lines)
                    w.WriteLine(line);
        }
    }

    [Require("Source.Include", "Uno/WinAPIHelper.h")]
    extern(WIN32) static class WinAPI
    {
        public static string GetLastErrorString()
        @{
            LPWSTR lpMsgBuf;
            FormatMessageW(
                FORMAT_MESSAGE_ALLOCATE_BUFFER |
                FORMAT_MESSAGE_FROM_SYSTEM |
                FORMAT_MESSAGE_IGNORE_INSERTS,
                nullptr,
                GetLastError(),
                MAKELANGID(LANG_ENGLISH, SUBLANG_DEFAULT),
                (LPWSTR)&lpMsgBuf, // Cast because callee is allocating buffer
                0, nullptr);

            uString* msg = uString::Utf16((const char16_t*) lpMsgBuf);
            LocalFree(lpMsgBuf);
            return msg;
        @}
    }
}
