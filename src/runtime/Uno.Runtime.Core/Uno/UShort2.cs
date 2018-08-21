// This file was generated based on lib/UnoCore/Source/Uno/UShort2.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct UShort2
    {
        public ushort X;
        public ushort Y;

        public UShort2(ushort xy)
        {
            this.X = this.Y = xy;
        }

        public UShort2(ushort x, ushort y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return (this.X.ToString() + ", ") + this.Y.ToString();
        }

        public ushort this[int index]
        {
            get
            {
                if (index == 0)
                    return this.X;
                else if (index == 1)
                    return this.Y;
                else
                    throw new global::System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    this.X = value;
                else if (index == 1)
                    this.Y = value;
                else
                    throw new global::System.IndexOutOfRangeException();
            }
        }

        public static Int2 operator +(UShort2 a, UShort2 b)
        {
            return new Int2(a.X + b.X, a.Y + b.Y);
        }

        public static Int2 operator -(UShort2 a, UShort2 b)
        {
            return new Int2(a.X - b.X, a.Y - b.Y);
        }

        public static Int2 operator *(UShort2 a, UShort2 b)
        {
            return new Int2(a.X * b.X, a.Y * b.Y);
        }

        public static Int2 operator /(UShort2 a, UShort2 b)
        {
            return new Int2(a.X / b.X, a.Y / b.Y);
        }

        public static Int2 operator +(UShort2 a, ushort b)
        {
            return new Int2(a.X + b, a.Y + b);
        }

        public static Int2 operator -(UShort2 a, ushort b)
        {
            return new Int2(a.X - b, a.Y - b);
        }

        public static Int2 operator *(UShort2 a, ushort b)
        {
            return new Int2(a.X * b, a.Y * b);
        }

        public static Int2 operator /(UShort2 a, ushort b)
        {
            return new Int2(a.X / b, a.Y / b);
        }

        public static bool operator ==(UShort2 a, UShort2 b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator !=(UShort2 a, UShort2 b)
        {
            return (a.X != b.X) || (a.Y != b.Y);
        }

        public static implicit operator UShort2(SByte2 v)
        {
            return new UShort2((ushort)v.X, (ushort)v.Y);
        }

        public static implicit operator UShort2(Byte2 v)
        {
            return new UShort2((ushort)v.X, (ushort)v.Y);
        }

        public static explicit operator UShort2(Short2 v)
        {
            return new UShort2((ushort)v.X, (ushort)v.Y);
        }

        public static explicit operator UShort2(Int2 v)
        {
            return new UShort2((ushort)v.X, (ushort)v.Y);
        }

        public static explicit operator UShort2(Float2 v)
        {
            return new UShort2((ushort)v.X, (ushort)v.Y);
        }
    }
}
