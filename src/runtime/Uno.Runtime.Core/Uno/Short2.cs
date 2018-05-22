// This file was generated based on lib/UnoCore/Source/Uno/Short2.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct Short2
    {
        public short X;
        public short Y;

        public Short2(short xy)
        {
            this.X = this.Y = xy;
        }

        public Short2(short x, short y)
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

        public short this[int index]
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

        public static Int2 operator +(Short2 a, Short2 b)
        {
            return new Int2(a.X + b.X, a.Y + b.Y);
        }

        public static Int2 operator -(Short2 a, Short2 b)
        {
            return new Int2(a.X - b.X, a.Y - b.Y);
        }

        public static Int2 operator *(Short2 a, Short2 b)
        {
            return new Int2(a.X * b.X, a.Y * b.Y);
        }

        public static Int2 operator /(Short2 a, Short2 b)
        {
            return new Int2(a.X / b.X, a.Y / b.Y);
        }

        public static Int2 operator +(Short2 a, short b)
        {
            return new Int2(a.X + b, a.Y + b);
        }

        public static Int2 operator -(Short2 a, short b)
        {
            return new Int2(a.X - b, a.Y - b);
        }

        public static Int2 operator *(Short2 a, short b)
        {
            return new Int2(a.X * b, a.Y * b);
        }

        public static Int2 operator /(Short2 a, short b)
        {
            return new Int2(a.X / b, a.Y / b);
        }

        public static bool operator ==(Short2 a, Short2 b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator !=(Short2 a, Short2 b)
        {
            return (a.X != b.X) || (a.Y != b.Y);
        }

        public static implicit operator Short2(SByte2 v)
        {
            return new Short2((short)v.X, (short)v.Y);
        }

        public static implicit operator Short2(Byte2 v)
        {
            return new Short2((short)v.X, (short)v.Y);
        }

        public static explicit operator Short2(UShort2 v)
        {
            return new Short2((short)v.X, (short)v.Y);
        }

        public static explicit operator Short2(Int2 v)
        {
            return new Short2((short)v.X, (short)v.Y);
        }

        public static explicit operator Short2(Float2 v)
        {
            return new Short2((short)v.X, (short)v.Y);
        }
    }
}
