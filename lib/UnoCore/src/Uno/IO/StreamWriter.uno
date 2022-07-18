using Uno.Compiler.ExportTargetInterop;
using Uno.Text;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.StreamWriter")]
    public class StreamWriter : TextWriter
    {
        private Stream _stream;
        private byte[] _buffer;
        private int _index;

        internal const int BufferSize = 256;

        public Stream BaseStream
        {
            get
            {
                return _stream;
            }
        }

        public StreamWriter(Stream stream)
        {
            _stream = stream;

            _buffer = new byte[BufferSize];
            _index = 0;
        }

        protected override void Dispose(bool disposing)
        {
            Flush();
            if (disposing && _stream != null)
            {
                _stream.Close();
            }
            base.Dispose(disposing);
        }

        public override void Close()
        {
            Dispose(true);
        }

        public override void Write(char value)
        {
            Write(Utf8.GetBytes(value.ToString()), 0, 1);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (index + count > buffer.Length)
            {
                throw new ArgumentException("range");
            }
            var array = Utf8.GetBytes(new string(buffer));
            Write(array, index, count);
        }

        public override void Write(string value)
        {
            var array = Utf8.GetBytes(value);
            Write(array, 0, array.Length);
        }

        private void Write(byte[] array, int index, int count)
        {
            if (_index + count >= BufferSize)
            {
                _stream.Write(_buffer, 0, _index);
                _index = 0;
            }
            if (count >= BufferSize)
            {
                _stream.Write(array, index, count);
                return;
            }
            for (var i = 0; i < count; i++)
            {
                _buffer[_index++] = array[index + i];
            }
        }

        public override void Flush()
        {
            if (_index != 0)
            {
                _stream.Write(_buffer, 0, _index);
                _index = 0;
            }
            _stream.Flush();
        }
    }
}
