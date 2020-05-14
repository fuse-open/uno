using Uno.Compiler;

namespace Uno
{
    public class Source
    {
        static Source _unknown;
        public static Source Unknown => _unknown ?? (_unknown = new Source(SourceFile.Unknown));

        public readonly SourceFile File;
        public SourcePackage Package => File.Package;
        
        public readonly int Line;
        public readonly int Column;
        public readonly int Length;

        public string FullPath => File.FullPath;
        public string Value => File.Text.Substring(Offset, Length);
        public bool IsUnknown => File.IsUnknown;
        public int Offset => File.GetOffset(Line, Column);
        public int EndOffset => Offset + Length;
        public int EndLine => Line + File.GetNewlineCount(Line, Column, Length);
        public int Part => File.GetPart(Line);
        
        public int EndColumn
        {
            get
            {
                var i = Value.LastIndexOf('\n');
                return i == -1
                    ? Column + Length
                    : Length - i;
            }
        }

        public Source(string fullPath, int line = 0, int column = 0, int length = 0)
            : this(new SourceFile(fullPath), line, column, length)
        {
        }

        public Source(SourcePackage upk, string fullPath, int line = 0, int column = 0, int length = 0)
            : this(new SourceFile(upk, fullPath), line, column, length)
        {
        }

        public Source(SourceFile file, int line = 0, int column = 0, int length = 0)
        {
            File = file;
            Line = line;
            Column = column;
            Length = length;
        }

        public override string ToString()
        {
            return Line > 0 && Column > 0
                    ? File + "(" + Line + "." + Column + ")" :
                Line > 0
                    ? File + "(" + Line + ")" 
                    : File.ToString();
        }
    }
}
