using System;

namespace Uno.Compiler
{
    public class ForeignItem
    {
        public enum Kind { Unknown, Java, CSource, CHeader, ObjCSource, ObjCHeader, Swift };
        public readonly string UnixPath;
        public readonly Kind SourceKind;
        public readonly string Condition;

        public ForeignItem(string path, Kind sourceKind, string condition)
        {
            UnixPath = path;
            SourceKind = sourceKind;
            Condition = condition;
        }

        public string GetCopyFileType()
        {
            switch (SourceKind)
            {
                case Kind.CSource:
                case Kind.ObjCSource:
                case Kind.Swift:
                    return "SourceFile";
                case Kind.CHeader:
                case Kind.ObjCHeader:
                    return "HeaderFile";
                default:
                    return null;
            }
        }

        public static ForeignItem FromString(string str)
        {
            var parts = str.Split(':');
            return new ForeignItem(
                    parts[0],
                    parts.Length > 1
                        ? (Kind)Enum.Parse(typeof(Kind), parts[1])
                        : 0,
                    parts.Length > 2
                        ? parts[2]
                        : null);
        }

        public override string ToString()
        {
            return UnixPath + ":" + SourceKind + (
                    !string.IsNullOrEmpty(Condition)
                        ? ":" + Condition
                        : null
                );
        }
    }
}
