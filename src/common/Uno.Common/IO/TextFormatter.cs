using System;
using System.IO;
using System.Text;

namespace Uno.IO
{
    public class TextFormatter : IDisposable
    {
        protected bool EnableMinify;

        readonly TextWriter _w;
        readonly string _indentValue;
        int _lineCount, _indent;
        bool _skip, _disableSkip;
        bool _disposed;

        public TextFormatter(TextWriter w, string indentValue = "    ")
        {
            _w = w;
            _indentValue = indentValue;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    Close();

                _disposed = true;
            }
        }

        public void Close()
        {
            _w.Close();
        }

        public void Begin(bool cond)
        {
            if (cond) Write("(");
        }

        public void End(bool cond)
        {
            if (cond) Write(")");
        }

        public void WriteWhen(bool cond, char ch)
        {
            if (cond) Write(ch);
        }

        public void WriteWhen(bool cond, string str)
        {
            if (cond) Write(str);
        }

        public void WriteWhen(bool cond, object str)
        {
            if (cond) Write(str);
        }

        public void Skip()
        {
            _skip = true;
        }

        public void Skip(bool skip)
        {
            _skip = skip;
        }

        public void DisableSkip()
        {
            _disableSkip = true;
        }

        public string IndentString
        {
            get
            {
                switch (_indent)
                {
                    case 0:
                        return "";
                    case 1:
                        return _indentValue;
                    case 2:
                        return _indentValue + _indentValue;
                    case 3:
                        return _indentValue + _indentValue + _indentValue;
                    default:
                    {
                        var sb = new StringBuilder();
                        for (int i = 0; i < _indent; i++)
                            sb.Append(_indentValue);
                        return sb.ToString();
                    }
                }
            }
        }

        public void Indent(int count = 1)
        {
            _indent += count;
        }

        public void Unindent(int count = 1)
        {
            _indent -= count;
        }

        public void Indent(string str)
        {
            WriteLine(str);
            _indent++;
        }

        public void Unindent(string str)
        {
            _indent--;
            WriteLine(str);
        }

        void WriteIndent()
        {
            if (EnableMinify)
                return;

            for (int i = 0; i < _indent; i++)
                _w.Write(_indentValue);
        }

        public void Write<T>(T str)
        {
            _w.Write(str);
        }

        public void WriteBlock(string str)
        {
            int start = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\n')
                {
                    _w.Write(str.Substring(start, 1 + i - start));
                    start = i + 1;
                    _lineCount++;
                    WriteIndent();
                }
            }

            _w.Write(str.Substring(start, str.Length - start));
        }

        public void EndLine()
        {
            if (!EnableMinify)
            {
                _w.WriteLine();
                _lineCount++;
            }
        }

        public void EndLine(string str)
        {
            _w.Write(str);
            EndLine();
        }

        public void BeginLine(bool indent = true)
        {
            if (_skip)
            {
                if (_lineCount > 0 && !_disableSkip)
                    EndLine();

                _skip = false;
            }

            if (_disableSkip)
                _disableSkip = false;

            if (indent)
                WriteIndent();
        }

        public void BeginLine(string str)
        {
            BeginLine();
            _w.Write(str);
        }

        public void WriteLine(string str)
        {
            BeginLine();
            _w.Write(str);
            EndLine();
        }

        public void WriteNonIndentedLine(string str)
        {
            BeginLine(false);
            _w.Write(str);
            EndLine();
        }

        public void WriteLines(string str)
        {
            BeginLine();
            WriteBlock(str);
            EndLine();
        }
    }
}
