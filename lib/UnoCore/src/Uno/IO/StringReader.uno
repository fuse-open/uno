using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.StringReader")]
    public class StringReader : TextReader
    {
        private string _string;
        private int _index;
        private int _length;

        public StringReader(string text)
        {
            _string = text;
            _index = 0;
            _length = text == null ? 0 : text.Length;
        }

        public override void Close()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
        }

        public override int Peek()
        {
            if (_string == null)
            {
                throw new ObjectDisposedException("StringReader");
            }
            if (_index == _length)
                return -1;
            return (int)_string[_index];
        }

        public override int Read()
        {
            if (_string == null)
            {
                throw new ObjectDisposedException("StringReader");
            }
            if (_index == _length)
                return -1;
            return (int)_string[_index++];
        }

        public override int Read(char[] buffer, int index, int count)
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
            if (_string == null)
            {
                throw new ObjectDisposedException("StringReader");
            }
            var readCount = _length - _index;
            if (readCount > 0)
            {
                if (readCount > count)
                    readCount = count;
                for (var i = 0; i < readCount; i++)
                {
                    buffer[index + i] = _string[_index + i];
                }
                _index += readCount;
            }
            return readCount;
        }

        public override string ReadToEnd()
        {
            if (_string == null)
            {
                throw new ObjectDisposedException("StringReader");
            }
            if (_index == 0)
                return _string;
            _index = _length;
            return _string.Substring(_index, _length - _index);
        }
    }
}
