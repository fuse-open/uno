using Uno.Text;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.StringWriter")]
    public class StringWriter : TextWriter
    {
        private bool _open;
        private StringBuilder _stringBuilder;

        private char[] _buffer;
        private int _index;

        internal const int BufferSize = 256;

        public StringWriter()
        {
            _stringBuilder = new StringBuilder();
            _open = true;

            _buffer = new char[BufferSize];
            _index = 0;
        }

        /*public StringWriter(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }*/

        protected override void Dispose(bool disposing)
        {
            _open = false;
            if (_index != 0)
                WriteBuffer();
            base.Dispose(disposing);
        }

        public override void Close()
        {
           Dispose(true);
           _open = false;
        }

        public override void Write(char value)
        {
            if (!_open)
            {
                throw new ObjectDisposedException("StringWriter");
            }
            _buffer[_index++] = value;
            if (_index == BufferSize)
            {
                WriteBuffer();
            }
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
            if (!_open)
            {
                throw new ObjectDisposedException("StringWriter");
            }
            if (_index + count >= BufferSize)
            {
                WriteBuffer();
            }
            if (count >= BufferSize)
            {
                char[] array = new char[count];
                Array.Copy(buffer, index, array, 0, count);
                _stringBuilder.Append(array);
            }
            else
            {
                Array.Copy(buffer, index, _buffer, _index, count);
                _index += count;
            }
        }

        private void WriteBuffer()
        {
            char[] array = new char[_index];
            Array.Copy(_buffer, 0, array, 0, _index);
            _stringBuilder.Append(array);
            _index = 0;
        }

        public override string ToString()
        {
            if (_index != 0)
            {
                WriteBuffer();
            }
            return _stringBuilder.ToString();
        }
    }
}

