using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Uno.IO;

namespace Uno.Compiler.API.Domain.Serialization
{
    public class CacheReader : BinaryReader
    {
        readonly SourcePackage _upk;
        readonly List<string> _strings = new List<string>();

        Source _lastSource = Source.Unknown;
        SourceFile _lastFile = SourceFile.Unknown;
        int _lastLine;
        int _lastColumn;
        int _lastLength;

        public CacheReader(SourcePackage upk, string filename)
            : base(File.OpenRead(filename), new UTF8Encoding())
        {
            _upk = upk;
        }

        public void VerifyMagic(uint magic)
        {
            if (ReadUInt32() != magic)
                throw new CacheException("Invalid magic number");
        }

        public void ReadList<T>(List<T> result, Func<CacheReader, T> read)
        {
            var c = Read7BitEncodedInt();
            while (c-- > 0)
                result.Add(read(this));
        }

        public string ReadGlobalString()
        {
            var flags = (StringFlags) ReadByte();
            switch (flags)
            {
                case StringFlags.Null:
                {
                    return null;
                }
                case StringFlags.Empty:
                {
                    return "";
                }
                case StringFlags.Index:
                {
                    return _strings[Read7BitEncodedInt()];
                }
                case StringFlags.BacktickSeparated:
                case StringFlags.ColonSeparated:
                case StringFlags.CommaSeparated:
                case StringFlags.DotSeparated:
                case StringFlags.DashSeparated:
                case StringFlags.LeftSeparated:
                case StringFlags.RightSeparated:
                case StringFlags.SlashSeparated:
                case StringFlags.SpaceSeparated:                    
                case StringFlags.SingleQuoteSeparated:
                case StringFlags.DoubleQuoteSeparated:
                case StringFlags.AssignSeparated:
                {
                    var result = string.Join(Separators[(int) flags], ReadGlobalStrings());
                    _strings.Add(result);
                    return result;
                }
                case StringFlags.String:
                {
                    var result = ReadString();
                    _strings.Add(result);
                    return result;
                }
                default:
                {
                    return ((char) flags).ToString();
                }
            }
        }

        public Source ReadSource()
        {
            bool flag;
            return ReadSource(out flag);
        }

        public Source ReadSource(out bool flag)
        {
            var flags = (SourceFlags) ReadByte();
            flag = (flags & SourceFlags.Flag) != 0;
            flags &= ~SourceFlags.Flag;

            switch (flags)
            {
                case 0:
                    return null;

                case SourceFlags.Marker:
                    return _lastSource;

                case SourceFlags.Marker | SourceFlags.Line:
                    _lastLine += Read7BitEncodedInt() - 32;
                    break;

                case SourceFlags.Marker | SourceFlags.Line | SourceFlags.Column:
                    _lastLine += Read7BitEncodedInt() - 32;
                    _lastColumn = Read7BitEncodedInt();
                    break;

                case SourceFlags.Marker | SourceFlags.Line | SourceFlags.Column | SourceFlags.Length:
                    _lastLine += Read7BitEncodedInt() - 32;
                    _lastColumn = Read7BitEncodedInt();
                    _lastLength = Read7BitEncodedInt();
                    break;

                case SourceFlags.Marker | SourceFlags.Line | SourceFlags.Length:
                    _lastLine += Read7BitEncodedInt() - 32;
                    _lastLength = Read7BitEncodedInt();
                    break;

                case SourceFlags.Marker | SourceFlags.Column | SourceFlags.Length:
                    _lastColumn = Read7BitEncodedInt();
                    _lastLength = Read7BitEncodedInt();
                    break;

                case SourceFlags.Marker | SourceFlags.Column:
                    _lastColumn = Read7BitEncodedInt();
                    break;

                default:
                    if ((flags & SourceFlags.MarkerMask) != SourceFlags.Marker)
                        throw new CacheException("Invalid marker");

                    if (flags.HasFlag(SourceFlags.Path))
                    {
                        var path = ReadGlobalString();
                        if (!string.IsNullOrEmpty(path) && path[0] != '<')
                            path = path.UnixToNative().ToFullPath(_upk.SourceDirectory);
                        _lastFile = new SourceFile(_upk, path);
                        _lastLine = 0;
                    }

                    if (flags.HasFlag(SourceFlags.Line))
                        _lastLine += Read7BitEncodedInt() - 32;
                    if (flags.HasFlag(SourceFlags.Column))
                        _lastColumn = Read7BitEncodedInt();
                    if (flags.HasFlag(SourceFlags.Length))
                        _lastLength = Read7BitEncodedInt();
                    break;
            }

            return _lastSource = new Source(_lastFile, _lastLine, _lastColumn, _lastLength);
        }

        public SourceValue ReadGlobalValue()
        {
            var src = ReadSource();
            var val = ReadGlobalString();
            return new SourceValue(src, val);
        }

        public string[] ReadGlobalStrings()
        {
            var result = new string[ReadCompressedInt()];
            for (int i = 0; i < result.Length; i++)
                result[i] = ReadGlobalString();
            return result;
        }

        public int ReadCompressedInt()
        {
            return Read7BitEncodedInt();
        }

        static readonly string[] Separators = GetSeparators();

        static string[] GetSeparators()
        {
            var result = new string[(int) StringFlags.Max];
            result[(int) StringFlags.SpaceSeparated] = " ";
            result[(int) StringFlags.SlashSeparated] = "/";
            result[(int) StringFlags.LeftSeparated] = "(";
            result[(int) StringFlags.RightSeparated] = ")";
            result[(int) StringFlags.DashSeparated] = "-";
            result[(int) StringFlags.DotSeparated] = ".";
            result[(int) StringFlags.CommaSeparated] = ",";
            result[(int) StringFlags.ColonSeparated] = ":";
            result[(int) StringFlags.BacktickSeparated] = "`";
            result[(int) StringFlags.SingleQuoteSeparated] = "'";
            result[(int) StringFlags.DoubleQuoteSeparated] = "\"";
            result[(int) StringFlags.AssignSeparated] = "=";
            return result;
        }
    }
}
