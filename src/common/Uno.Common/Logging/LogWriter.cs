using System.IO;
using System.Text;

namespace Uno.Logging
{
    public class LogWriter : TextWriter
    {
        readonly LogState _state;
        public readonly TextWriter Inner;

        public override Encoding Encoding => Inner.Encoding;
        public bool IsNull => Inner == Null;

        internal LogWriter(LogState state, TextWriter w)
        {
            _state = state;
            Inner = (w as LogWriter)?.Inner ?? w;
        }

        public override void Write(char[] buffer, int index, int count)
        {
            lock (_state)
            {
                _state.Flush(Inner);
                Inner.Write(buffer, index, count);
            }
        }

        public override void Write(char c)
        {
            lock (_state)
            {
                _state.Flush(Inner);
                Inner.Write(c);
            }
        }

        public override void Write(string str)
        {
            lock (_state)
            {
                _state.Flush(Inner);
                Inner.Write(str);
            }
        }
        
        public override void Write(object obj)
        {
            lock (_state)
            {
                _state.Flush(Inner);
                Inner.Write(obj);
            }
        }

        public override void WriteLine(string str)
        {
            lock (_state)
            {
                _state.Flush(Inner);
                Inner.WriteLine(str);
            }
        }

        public override void WriteLine(object obj)
        {
            lock (_state)
            {
                _state.Flush(Inner);
                Inner.WriteLine(obj);
            }
        }

        public override void WriteLine()
        {
            lock (_state)
            {
                _state.Flush(Inner);
                Inner.WriteLine();
            }
        }
    }
}