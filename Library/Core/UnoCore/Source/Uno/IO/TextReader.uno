using Uno.Text;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.TextReader")]
    public abstract class TextReader : IDisposable
    {
        internal const int BufferSize = 4096;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual void Close()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual int Peek()
        {
            return -1;
        }

        public virtual int Read()
        {
            return -1;
        }

        public virtual int Read(char[] buffer, int index, int count)
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
            int i = 0;
            for (i = 0; i < count; i++)
            {
                int symbol = Read();
                if (symbol == -1)
                {
                    break;
                }
                buffer[index + i] = (char)symbol;
            }
            return i;
        }

        public virtual string ReadToEnd()
        {
            char[] buffer = new char[BufferSize];
            StringBuilder stringBuilder = new StringBuilder();
            int count;
            while ((count = this.Read(buffer, 0, buffer.Length)) != 0)
            {
                char[] array = new char[count];
                Array.Copy(buffer, array, count);
                stringBuilder.Append(array);
            }
            return stringBuilder.ToString();
        }

        public virtual int ReadBlock(char[] buffer, int index, int count)
        {
            int i = 0;
            int readCount;
            do
            {
                i += (readCount = this.Read(buffer, index + i, count - i));
            }
            while (readCount > 0 && i < count);
            return i;
        }

        public virtual string ReadLine()
        {
            List<char> buffer = new List<char>(16);
            int symbol = 0;
            while (true)
            {
                symbol = Read();
                if (symbol == -1 || symbol == (int)'\r' || symbol == (int)'\n')
                {
                    break;
                }
                buffer.Add((char)symbol);
            }
            if (symbol == (int)'\r' && Peek() == (int)'\n')
            {
                Read();
            }

            if (buffer.Count == 0 && symbol == -1)
                return null;

            return new string(buffer.ToArray());
        }
    }
}
