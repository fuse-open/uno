using System;

namespace Uno.UX.Markup
{
    public sealed class FileSourceInfo : IEquatable<FileSourceInfo>
    {
        public readonly string FileName;
        public readonly int LineNumber;

        public FileSourceInfo(string fileName, int lineNumber)
        {
            FileName = fileName;
            LineNumber = lineNumber;
        }

        public static FileSourceInfo Unknown => new FileSourceInfo("(unknown file)", 0);

        public bool Equals(FileSourceInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(FileName, other.FileName) && LineNumber == other.LineNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as FileSourceInfo);
        }

        public override int GetHashCode()
        {
            return (FileName?.GetHashCode() ?? 0) ^ LineNumber;
        }
    }
}
