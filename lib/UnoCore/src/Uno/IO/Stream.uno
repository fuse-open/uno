using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.Stream")]
    public abstract class Stream : IDisposable
    {
        public abstract bool CanRead
        {
            get;
        }

        public abstract bool CanWrite
        {
            get;
        }

        public abstract bool CanSeek
        {
            get;
        }

        public virtual bool CanTimeout
        {
            get { return false; }
        }

        public virtual int ReadTimeout
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public virtual int WriteTimeout
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public abstract long Length
        {
            get;
        }

        public abstract long Position
        {
            get;
            set;
        }

        public abstract void SetLength(long value);

        public abstract int Read(byte[] dst, int byteOffset, int byteCount);
        public abstract void Write(byte[] src, int byteOffset, int byteCount);
        public abstract long Seek(long byteOffset, SeekOrigin origin);
        public abstract void Flush();

        public virtual void Dispose(bool disposing)
        {
        }

        public virtual void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
