using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    [TargetSpecificType]
    [Set("TypeName", "FILE*")]
    [Set("Include", "cstdio")]
    extern(CPLUSPLUS)
    struct FILEPtr
    {
        [extern(WIN32) Require("Source.Include", "Uno/WinAPIHelper.h")]
        public static FILEPtr OpenOrThrow(string filename, string mode)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            if defined(WIN32)
            @{
                FILE* retval;
                if (_wfopen_s(&retval, (const wchar_t*) $0->Ptr(), (const wchar_t*) $1->Ptr()) != 0)
                    @{Throw(string):Call($0)};
                return retval;
            @}
            else
            @{
                FILE* retval = fopen(uCString($0).Ptr, uCString($1).Ptr);
                if (!retval)
                    @{Throw(string):Call($0)};
                return retval;
            @}
        }

        static void Throw(string filename)
        {
            throw new FileNotFoundException("Can't open file: " + filename, filename);
        }
    }

    [DotNetType("System.IO.FileStream")]
    public class FileStream : Stream
    {
        extern(CPLUSPLUS) FILEPtr _fp;
        bool _canRead, _canWrite;

        public FileStream(string filename, FileMode mode)
        {
            if defined(CPLUSPLUS)
                _fp = FILEPtr.OpenOrThrow(filename, GetNativeFileMode(filename, mode));
            else
                throw new NotImplementedException();
        }

        extern(CPLUSPLUS)
        internal FileStream(FILEPtr fp, bool canRead, bool canWrite)
        {
            _fp = fp;
            _canRead = canRead;
            _canWrite = canWrite;
        }

        extern(CPLUSPLUS)
        string GetNativeFileMode(string filename, FileMode mode)
        {
            switch (mode)
            {
                case FileMode.Truncate:
                {
                    if (!File.Exists(filename))
                        throw new FileNotFoundException("File not found: " + filename, filename);

                    _canRead = true;
                    _canWrite = true;
                    extern(FILEPtr.OpenOrThrow(filename, "wb"))
                        "fclose($0)";
                    return "r+b";
                }
                case FileMode.Create:
                {
                    _canRead = true;
                    _canWrite = true;
                    extern(FILEPtr.OpenOrThrow(filename, "wb"))
                        "fclose($0)";
                    return "r+b";
                }
                case FileMode.CreateNew:
                {
                    if (File.Exists(filename))
                        throw new IOException("File already exists: " + filename);

                    _canWrite = true;
                    return "wb";
                }
                case FileMode.OpenOrCreate:
                {
                    if (!File.Exists(filename))
                        extern(FILEPtr.OpenOrThrow(filename, "wb"))
                            "fclose($0)";

                    _canRead = true;
                    _canWrite = true;
                    return "r+b";
                }
                case FileMode.Open:
                    _canRead = true;
                    _canWrite = true;
                    return "r+b";
                case FileMode.Append:
                    _canWrite = true;
                    return "ab";
            }

            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        public override bool CanRead
        {
            get { return _canRead; }
        }

        public override bool CanWrite
        {
            get { return _canWrite; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override long Length
        {
            get
            {
                CheckDisposed();
                if defined(CPLUSPLUS)
                @{
                    long p = ftell(@{$$._fp});
                    fseek(@{$$._fp}, 0, SEEK_END);
                    long l = ftell(@{$$._fp});
                    fseek(@{$$._fp}, p, SEEK_SET);
                    return l;
                @}
                else
                    throw new NotImplementedException();
            }
        }

        public override long Position
        {
            get
            {
                CheckDisposed();
                if defined(CPLUSPLUS)
                    return extern<long> "ftell(@{$$._fp})";
                else
                    throw new NotImplementedException();
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] dst, int byteOffset, int byteCount)
        {
            CheckDisposed();

            // The stream shall be flushed (fflush) or repositioned (fseek, fsetpos, rewind) before a reading operation that follows a writing operation.
            if (_canRead && _canWrite)
                Flush();
            else if (!_canRead)
                throw new IOException("File stream is not readable");

            if defined(CPLUSPLUS)
                return extern<int> "(int)fread((uint8_t*)$0->Ptr() + $1, 1, $2, @{$$._fp})";
            else
                throw new NotImplementedException();
        }

        public override void Write(byte[] src, int byteOffset, int byteCount)
        {
            CheckDisposed();

            // The stream shall be repositioned (fseek, fsetpos, rewind) before a writing operation that follows a reading operation (whenever that operation did not reach the end-of-file).
            if (_canRead && _canWrite)
                Seek(0, SeekOrigin.Current);
            else if (!_canWrite)
                throw new IOException("File stream is not writable");

            if defined(CPLUSPLUS)
            {
                int result = extern<int> "(int)fwrite((const uint8_t*)$0->Ptr() + $1, 1, $2, @{$$._fp})";

                if (result != byteCount)
                    throw new IOException("Error while writing to file stream");
            }
            else
                throw new NotImplementedException();
        }

        public override long Seek(long byteOffset, SeekOrigin origin)
        {
            if defined(CPLUSPLUS)
            @{
                switch ($1)
                {
                case @{SeekOrigin.Begin}:
                    if (fseek(@{$$._fp}, (long)$0, SEEK_SET) == 0)
                        return ftell(@{$$._fp});
                    break;

                case @{SeekOrigin.Current}:
                    if (fseek(@{$$._fp}, (long)$0, SEEK_CUR) == 0)
                        return ftell(@{$$._fp});
                    break;

                case @{SeekOrigin.End}:
                    if (fseek(@{$$._fp}, (long)$0, SEEK_END) == 0)
                        return ftell(@{$$._fp});
                    break;
                }
            @}

            throw new IOException("Error while seeking in file stream");
        }

        public override void Flush()
        {
            CheckDisposed();
            if defined(CPLUSPLUS)
                extern "fflush(@{$$._fp})";
        }

        public override void Dispose(bool disposing)
        {
            if defined(CPLUSPLUS)
            @{
                if (!@{$$._fp})
                    return;
                fclose(@{$$._fp});
                @{$$._fp} = nullptr;
            @}
        }

        void CheckDisposed()
        {
            if defined(CPLUSPLUS)
                if (!extern<bool> "@{$$._fp}")
                    throw new ObjectDisposedException("The file stream was closed");
        }
    }
}
