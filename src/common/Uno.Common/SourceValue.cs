using System;

namespace Uno
{
    public struct SourceValue : IComparable<SourceValue>
    {
        public readonly Source Source;
        public readonly string String;

        public SourceValue(Source src, string value)
        {
            Source = src;
            String = value;
        }

        public override int GetHashCode()
        {
            return (String ?? "").GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is SourceValue && this == (SourceValue) obj;
        }

        public override string ToString()
        {
            return String;
        }

        public int CompareTo(SourceValue other)
        {
            return string.Compare(String, other.String, StringComparison.InvariantCulture);
        }

        public static bool operator ==(SourceValue left, SourceValue right)
        {
            return left.String == right.String;
        }

        public static bool operator !=(SourceValue left, SourceValue right)
        {
            return left.String != right.String;
        }

        public static implicit operator SourceValue(string s)
        {
            return new SourceValue(Source.Unknown, s);
        }
    }
}