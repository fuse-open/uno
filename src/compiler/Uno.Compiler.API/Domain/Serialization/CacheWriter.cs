using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Uno.IO;

namespace Uno.Compiler.API.Domain.Serialization
{
    public class CacheWriter : BinaryWriter
    {
        public bool OptimizeSources;

        readonly SourcePackage _upk;
        readonly List<string> _strings = new List<string>();
        readonly Dictionary<string, int> _stringIndices = new Dictionary<string, int>();

        string _lastPath;
        int _lastLine;
        int _lastColumn;
        int _lastLength;

        public CacheWriter(SourcePackage upk, string filename)
            : base(File.Open(filename, FileMode.Create), new UTF8Encoding())
        {
            _upk = upk;
        }

        public void WriteGlobals(IReadOnlyList<string> l)
        {
            WriteCompressed(l.Count);
            foreach (var e in l)
                WriteGlobal(e);
        }

        public void WriteList<T>(List<T> list, Action<CacheWriter, T> write)
        {
            if (list == null)
            {
                Write7BitEncodedInt(0);
                return;
            }

            Write7BitEncodedInt(list.Count);
            foreach (var item in list)
                write(this, item);
        }

        public void WriteGlobal(string str)
        {
            if (str == null)
            {
                Write((byte) StringFlags.Null);
                return;
            }

            if (str.Length == 0)
            {
                Write((byte) StringFlags.Empty);
                return;
            }

            if (str.Length == 1 && str[0] >= ' ' && str[0] < 0xFF)
            {
                Write((byte) str[0]);
                return;
            }

            var sep = (char) 0x7f;
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                switch (c)
                {
                    case '\n':
                    {
                        Write((byte) StringFlags.String);
                        Write(str);
                        _strings.Add(str);
                        return;
                    }
                    case ' ':
                    case '/':
                    case '(':
                    case ')':
                    case '-':
                    case '.':
                    case ',':
                    case ':':
                    case '`':
                    case '\'':
                    case '"':
                    case '=':
                        if (c < sep)
                            sep = c;
                        break;
                }
            }

            int index;
            if (_stringIndices.TryGetValue(str, out index))
            {
                Write((byte) StringFlags.Index);
                Write7BitEncodedInt(index);
                return;
            }

            if (Separators[sep] != 0)
            {
                Write((byte) Separators[sep]);
                WriteGlobals(str.Split(sep));
            }
            else
            {
                Write((byte) StringFlags.String);
                Write(str);
            }

            _stringIndices.Add(str, _strings.Count);
            _strings.Add(str);
        }

        public void Write(Source s, bool flag = false)
        {
            var flags = SourceFlags.Marker;

            if (s == null)
                flags = 0;
            else
            {
                if (s.FullPath != _lastPath)
                {
                    _lastPath = s.FullPath;
                    flags |= SourceFlags.Path;
                }

                if (s.Line != _lastLine)
                    flags |= SourceFlags.Line;

                if (!OptimizeSources)
                {
                    if (s.Column != _lastColumn)
                    {
                        _lastColumn = s.Column;
                        flags |= SourceFlags.Column;
                    }

                    if (s.Length != _lastLength)
                    {
                        _lastLength = s.Length;
                        flags |= SourceFlags.Length;
                    }
                }
            }

            if (flag)
                flags |= SourceFlags.Flag;

            Write((byte) flags);

            if (flags.HasFlag(SourceFlags.Path))
            {
                WriteGlobal(s.FullPath.ToRelativePath(_upk.SourceDirectory).NativeToUnix());
                _lastLine = 0;
            }
            if (flags.HasFlag(SourceFlags.Line))
            {
                Write7BitEncodedInt(32 + s.Line - _lastLine);
                _lastLine = s.Line;
            }
            if (flags.HasFlag(SourceFlags.Column))
                Write7BitEncodedInt(s.Column);
            if (flags.HasFlag(SourceFlags.Length))
                Write7BitEncodedInt(s.Length);
        }

        public new void Write(string s)
        {
            base.Write(s);
        }

        public void WriteGlobal(SourceValue e)
        {
            Write(e.Source);
            WriteGlobal(e.String);
        }

        public void WriteCompressed(int i)
        {
            Write7BitEncodedInt(i);
        }

        static readonly StringFlags[] Separators = GetSeparators();

        static StringFlags[] GetSeparators()
        {
            var result = new StringFlags[0x80];
            result[' '] = StringFlags.SpaceSeparated;
            result['/'] = StringFlags.SlashSeparated;
            result['('] = StringFlags.LeftSeparated;
            result[')'] = StringFlags.RightSeparated;
            result['-'] = StringFlags.DashSeparated;
            result['.'] = StringFlags.DotSeparated;
            result[','] = StringFlags.CommaSeparated;
            result[':'] = StringFlags.ColonSeparated;
            result['`'] = StringFlags.BacktickSeparated;
            result['\''] = StringFlags.SingleQuoteSeparated;
            result['"'] = StringFlags.DoubleQuoteSeparated;
            result['='] = StringFlags.AssignSeparated;
            return result;
        }
    }
}