using System;
using Uno.Compiler;

namespace Uno.ProjectFormat
{
    public class IncludeItem : IComparable<IncludeItem>
    {
        public Source Source;
        public IncludeItemType Type;
        public string Value;
        public string Condition;

        public IncludeItem(string value, IncludeItemType type = IncludeItemType.Glob)
            : this(Source.Unknown, type, value)
        {
        }

        public IncludeItem(Source src, IncludeItemType type, string value = null, string condition = null)
        {
            // Deperated item type
            if ((int)type >= 100)
                type -= 100;

            Source = src;
            Type = type;
            Value = value;
            Condition = condition;
        }

        public int CompareTo(IncludeItem other)
        {
            return string.Compare(Value, other.Value, StringComparison.InvariantCulture);
        }

        public static IncludeItem FromString(Source src, string str)
        {
            var parts = str.PathSplit();

            switch (parts.Length)
            {
                case 1:
                    return new IncludeItem(src, IncludeItemType.Glob, parts[0]);
                case 2:
                case 3:
                {
                    IncludeItemType type;
                    if (!Enum.TryParse(parts[1], out type))
                        throw new ArgumentException("Invalid file type (" + parts[1] + ")");
                    return new IncludeItem(src, type, parts[0], parts.Length == 3 ? parts[2] : null);
                }
                default:
                    throw new ArgumentException("Invalid arguments provided for 'FILENAME[:TYPE[:CONDITION]]'");
            }
        }

        public override string ToString()
        {
            return Value.NativeToUnix() + (
                    Type != IncludeItemType.Glob
                        ? ":" + Type
                        : null
                ) + (
                    !string.IsNullOrEmpty(Condition)
                        ? ":" + Condition
                        : null
                );
        }

        public static bool IsForeignIncludeType(IncludeItemType type)
        {
            switch (type)
            {
                case IncludeItemType.Java:
                case IncludeItemType.JavaFile:
                case IncludeItemType.CHeader:
                case IncludeItemType.CHeaderFile:
                case IncludeItemType.CSource:
                case IncludeItemType.CSourceFile:
                case IncludeItemType.ObjCHeader:
                case IncludeItemType.ObjCSource:
                case IncludeItemType.Swift:
                    return true;
                default:
                    return false;
            }
        }

        public ForeignItem.Kind ForeignSourceKind
        {
            get
            {
                switch (Type)
                {
                    case IncludeItemType.Java:
                    case IncludeItemType.JavaFile:
                        return ForeignItem.Kind.Java;
                    case IncludeItemType.CHeader:
                    case IncludeItemType.CHeaderFile:
                        return ForeignItem.Kind.CHeader;
                    case IncludeItemType.CSource:
                    case IncludeItemType.CSourceFile:
                        return ForeignItem.Kind.CSource;
                    case IncludeItemType.ObjCHeader:
                        return ForeignItem.Kind.ObjCHeader;
                    case IncludeItemType.ObjCSource:
                        return ForeignItem.Kind.ObjCSource;
                    case IncludeItemType.Swift:
                        return ForeignItem.Kind.Swift;
                    default:
                        throw new InvalidOperationException("Not a foreign include item");
                }
            }
        }
    }
}