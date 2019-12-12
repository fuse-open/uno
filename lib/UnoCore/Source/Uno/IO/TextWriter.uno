using Uno.Text;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.IO
{
    [extern(DOTNET) DotNetType("System.IO.TextWriter")]
    public abstract class TextWriter : IDisposable
    {
        string _newline = Environment.NewLine;
        public virtual string NewLine
        {
            get { return _newline; }
            set { _newline = value ?? Environment.NewLine; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Flush()
        {
        }

        public virtual void Write(char value)
        {
        }

        public virtual void Write(char[] buffer, int index, int count)
        {
        }

        public virtual void Write(char[] buffer)
        {
            if (buffer != null)
            {
                Write(buffer, 0, buffer.Length);
            }
        }

        public virtual void Write(bool value)
        {
            Write(value.ToString());
        }

        public virtual void Write(int value)
        {
            Write(value.ToString());
        }

        public virtual void Write(uint value)
        {
            Write(value.ToString());
        }

        public virtual void Write(long value)
        {
            Write(value.ToString());
        }

        public virtual void Write(ulong value)
        {
            Write(value.ToString());
        }

        public virtual void Write(float value)
        {
            Write(value.ToString());
        }

        public virtual void Write(double value)
        {
            Write(value.ToString());
        }

        public virtual void Write(string value)
        {
            if (value != null)
            {
                Write(value.ToCharArray());
            }
        }

        public virtual void Write(object value)
        {
            if (value != null)
            {
                Write(value.ToString());
            }
        }

        public virtual void Write(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        public virtual void WriteLine()
        {
            Write(NewLine);
        }

        public virtual void WriteLine(char value)
        {
            Write(value);
            WriteLine();
        }

        public virtual void WriteLine(char[] buffer)
        {
            Write(buffer);
            WriteLine();
        }

        public virtual void WriteLine(char[] buffer, int index, int count)
        {
            Write(buffer, index, count);
            WriteLine();
        }

        public virtual void WriteLine(bool value)
        {
            Write(value);
            WriteLine();
        }

        public virtual void WriteLine(int value)
        {
            Write(value);
            WriteLine();
        }

        public virtual void WriteLine(uint value)
        {
            Write(value);
            WriteLine();
        }

        public virtual void WriteLine(long value)
        {
            Write(value);
            WriteLine();
        }

        public virtual void WriteLine(ulong value)
        {
            Write(value);
            WriteLine();
        }

        public virtual void WriteLine(float value)
        {
            Write(value);
            WriteLine();
        }

        public virtual void WriteLine(double value)
        {
            Write(value);
            WriteLine();
        }

        public virtual void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }

        public virtual void WriteLine(object value)
        {
            if (value == null)
            {
                WriteLine();
                return;
            }
            WriteLine(value.ToString());
        }

        public virtual void WriteLine(string format, params object[] args)
        {
            Write(string.Format(format, args));
            WriteLine();
        }
    }
}
