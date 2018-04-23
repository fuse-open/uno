using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.MemoryStream")]
    public class MemoryStream : Stream
    {
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return _canWrite; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanTimeout
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _length; }
        }

        public int Capacity
        {
            get { return _buffer.Length; }
        }

        public override long Position
        {
            get; set;
        }

        bool _canWrite = true;
        bool _canResize = true;
        byte[] _buffer = new byte[0];
        int _nextIncrease = 256;
        long _length;

        public MemoryStream()
        {
        }

        public MemoryStream(byte[] buffer) : this(buffer, true)
        {
        }

        public MemoryStream(byte[] buffer, bool writable)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            _buffer = buffer;
            _length = _buffer.Length;
            _canResize = false;
            _canWrite = writable;
        }

        public override void Write(byte[] src, int byteOffset, int byteCount)
        {
            if (!_canWrite)
                throw new NotSupportedException();

            EnsureCapacity(byteCount);
            for(int i = byteOffset; i < byteOffset + byteCount; i++)
            {
                _buffer[(int)Position] = src[i];
                Position += 1;
            }
            if (Position > Length)
            {
                _length = Position;
            }
        }

        public override int Read(byte[] dst, int byteOffset, int byteCount)
        {
            int i = 0;
            for(; i < byteCount && Position + i < Length; i++)
            {
                dst[i + byteOffset] = _buffer[(int)Position + i];
            }
            Position += i;
            return i;
        }

        public override long Seek(long byteOffset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = byteOffset;
                    break;
                case SeekOrigin.End:
                    Position = Length + byteOffset;
                    break;
                default:
                    Position = Position + byteOffset;
                    break;
            }
            return Position;
        }

        private void EnsureCapacity(int byteCount)
        {
            if (Position + byteCount <= Capacity)
            {
                return;
            }
            else if (Position + byteCount <= Capacity + _nextIncrease)
            {
                ResizeTo(Capacity + _nextIncrease);
            }
            else
            {
                ResizeTo((int)Position + byteCount);
            }
        }

        private void ResizeTo(int newSize)
        {
            if (!_canResize)
                throw new NotSupportedException();

            var newBuffer = new byte[newSize];
            Array.Copy(_buffer, newBuffer, _buffer.Length);
            _buffer = newBuffer;
            _nextIncrease = Capacity;
        }

        public virtual byte[] GetBuffer()
        {
            return _buffer;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
        }
    }
}
