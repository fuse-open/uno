using System.Collections.Generic;
using System.IO;
using Uno.IO;

namespace Uno.Compiler
{
    public class SourceFile
    {
        static SourceFile _unknown;
        public static SourceFile Unknown => _unknown ?? (_unknown = new SourceFile("(unknown)"));

        readonly List<int> _lineOffsets = new List<int>();
        readonly List<int> _lineParts = new List<int> {1};

        public readonly SourcePackage Package;
        public readonly string FullPath;
        public readonly bool IsUnknown;
        public readonly int StartLine;
        public readonly int StartColumn;

        // Lazy load text
        string _text;
        public string Text => _text ?? (_text = !IsUnknown && File.Exists(FullPath) 
                                            ? File.ReadAllText(FullPath) 
                                            : "(unknown file)");

        public SourceFile(string fullPath, string text = null, int line = 1, int column = 1)
            : this(SourcePackage.Unknown, fullPath, text, line, column)
        {
        }

        public SourceFile(SourcePackage upk, string fullPath, string text = null, int line = 1, int column = 1)
        {
            Package = upk;
            FullPath = fullPath;
            IsUnknown = !fullPath.IsValidPath() || !Path.IsPathRooted(fullPath);
            _text = text;
            StartLine = line;
            StartColumn = column;
        }

        public void AddPart(int line)
        {
            var last = _lineParts.Last();
            for (var i = _lineParts.Count; i <= line; i++)
                _lineParts.Add(last);
            _lineParts[line] = last + 1;
        }

        public int GetPart(int line)
        {
            return line < _lineParts.Count
                ? _lineParts[line]
                : _lineParts.Last();
        }

        public int GetOffset(int line)
        {
            // Calculate offsets on demand
            if (_lineOffsets.Count == 0)
            {
                for (var i = 0; i <= StartLine; i++)
                    _lineOffsets.Add(0);

                for (var i = 0; i < Text.Length; i++)
                    if (Text[i] == '\n')
                        _lineOffsets.Add(i + 1);
            }

            return line < _lineOffsets.Count
                ? _lineOffsets[line]
                : Text.Length;
        }

        public int GetOffset(int line, int column)
        {
            return GetOffset(line) + (
                column > 0
                    ? column - 1 
                    : 0);
        }

        public int GetNewlineCount(int line, int column, int length)
        {
            var start = GetOffset(line, column);
            var end = start + length;
            var text = Text;
            var count = 0;
            
            for (var i = start; i < end; i++)
                if (text[i] == '\n')
                    count++;

            return count;
        }

        public Source GetSourceFromOffsets(int startOffset, int endOffset)
        {
            // Calculate offsets on demand
            if (_lineOffsets.Count == 0)
                GetOffset(0);

            var line = 0;
            for (var l = _lineOffsets.Count - 1; line < l; line++)
                if (_lineOffsets[line + 1] > startOffset)
                    break;

            var column = startOffset - GetOffset(line) + 1;
            return new Source(this, line, column, endOffset - startOffset);
        }

        public override string ToString()
        {
            return FullPath.ToRelativePath();
        }
    }
}