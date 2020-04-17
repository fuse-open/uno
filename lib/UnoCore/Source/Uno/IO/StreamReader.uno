using Uno.Compiler.ExportTargetInterop;
using Uno.Text;
using Uno.Math;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.StreamReader")]
    public class StreamReader : TextReader
    {
        private Stream _stream;
        private Decoder _decoder;

        private byte[] _byteBuffer;
        private int _byteLen;

        private char[] _charBuffer;
        private int _charPos, _charLen;

        internal new const int BufferSize = 256;

        public Stream BaseStream
        {
            get
            {
                return _stream;
            }
        }

        public bool EndOfStream
        {
            get
            {
                if (_stream == null)
                    throw new ObjectDisposedException("StreamReader");
                if (_charPos < _charLen)
                    return false;
                ReadBuffer();
                return _charLen == 0;
            }
        }

        public StreamReader(Stream stream)
        {
            _stream = stream;
            _decoder = Encoding.UTF8.GetDecoder();

            _byteBuffer = new byte[BufferSize];
            _byteLen = 0;

            _charBuffer = new char[BufferSize];
            _charPos = 0;
            _charLen = 0;
        }

        public override void Close()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _stream != null)
            {
                _stream.Close();
                _stream = null;
            }
            base.Dispose(disposing);
        }

        public override int Peek()
        {
            if (_stream == null)
                throw new ObjectDisposedException("StreamReader");
            if (_charPos == _charLen)
            {
                ReadBuffer();
                if (_charLen == 0)
                    return -1;
            }
            return (int)_charBuffer[_charPos];
        }

        public override int Read()
        {
            if (_stream == null)
                throw new ObjectDisposedException("StreamReader");
            if (_charPos == _charLen)
            {
                ReadBuffer();
                if (_charLen == 0)
                    return -1;
            }
            return _charBuffer[_charPos++];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (index + count > buffer.Length)
                throw new ArgumentException("range");
            if (_stream == null)
                throw new ObjectDisposedException("StreamReader");

            if (_charLen == 0)
                ReadBuffer();

            var charsCount = 0;
            while (charsCount != count && _charLen != 0)
            {
                var symbols = Min(_charLen - _charPos, count - charsCount);
                for (var i = 0; i < symbols; ++i)
                {
                    buffer[index + charsCount + i] = _charBuffer[_charPos];
                    ++_charPos;
                }
                charsCount += symbols;
                if (charsCount != count)
                    ReadBuffer();
            }
            return charsCount;
        }

        public override string ReadToEnd()
        {
            var stringBuilder = new StringBuilder();
            if (_charLen == 0)
                ReadBuffer();
            while (_charLen != 0)
            {
                var count = _charLen - _charPos;
                var array = new char[count];
                for (var i = 0; i < count; i++)
                    array[i] = _charBuffer[_charPos + i];
                stringBuilder.Append(array);
                ReadBuffer();
            }
            return stringBuilder.ToString();
        }

        void ReadBuffer()
        {
            _charPos = 0;
            _charLen = 0;

            do {
                _byteLen = _stream.Read(_byteBuffer, 0, _byteBuffer.Length);
                if (_byteLen == 0)
                    return;

                _charLen += _decoder.GetChars(_byteBuffer, 0, _byteLen,
                                              _charBuffer, _charLen);
            } while (_charLen == 0);
        }
    }
}
