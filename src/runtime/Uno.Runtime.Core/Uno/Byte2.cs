// This file was generated based on lib/UnoCore/Source/Uno/Byte2.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct Byte2
    {
        public byte X;
        public byte Y;

        public Byte2(byte xy)
        {
            this.X = xy;
            this.Y = xy;
        }

        public Byte2(byte x, byte y)
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

        public byte this[int index]
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

        public static Int2 operator +(Byte2 a, Byte2 b)
        {
            return new Int2(a.X + b.X, a.Y + b.Y);
        }

        public static Int2 operator -(Byte2 a, Byte2 b)
        {
            return new Int2(a.X - b.X, a.Y - b.Y);
        }

        public static Int2 operator *(Byte2 a, Byte2 b)
        {
            return new Int2(a.X * b.X, a.Y * b.Y);
        }

        public static Int2 operator /(Byte2 a, Byte2 b)
        {
            return new Int2(a.X / b.X, a.Y / b.Y);
        }

        public static Int2 operator +(Byte2 a, byte b)
        {
            return new Int2(a.X + b, a.Y + b);
        }

        public static Int2 operator -(Byte2 a, byte b)
        {
            return new Int2(a.X - b, a.Y - b);
        }

        public static Int2 operator *(Byte2 a, byte b)
        {
            return new Int2(a.X * b, a.Y * b);
        }

        public static Int2 operator /(Byte2 a, byte b)
        {
            return new Int2(a.X / b, a.Y / b);
        }

        public static bool operator ==(Byte2 a, Byte2 b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator !=(Byte2 a, Byte2 b)
        {
            return (a.X != b.X) || (a.Y != b.Y);
        }

        public static explicit operator Byte2(SByte2 v)
        {
            return new Byte2((byte)v.X, (byte)v.Y);
        }

        public static explicit operator Byte2(Short2 v)
        {
            return new Byte2((byte)v.X, (byte)v.Y);
        }

        public static explicit operator Byte2(UShort2 v)
        {
            return new Byte2((byte)v.X, (byte)v.Y);
        }

        public static explicit operator Byte2(Int2 v)
        {
            return new Byte2((byte)v.X, (byte)v.Y);
        }

        public static explicit operator Byte2(Float2 v)
        {
            return new Byte2((byte)v.X, (byte)v.Y);
        }
    }
}
