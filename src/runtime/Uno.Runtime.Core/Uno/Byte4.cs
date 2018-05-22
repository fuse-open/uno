// This file was generated based on lib/UnoCore/Source/Uno/Byte4.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct Byte4
    {
        public byte X;
        public byte Y;
        public byte Z;
        public byte W;

        public Byte4(byte x, byte y, byte z, byte w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Byte4(Byte2 xy, byte z, byte w)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
            this.W = w;
        }

        public Byte4(byte x, Byte2 yz, byte w)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
            this.W = w;
        }

        public Byte4(byte x, byte y, Byte2 zw)
        {
            this.X = x;
            this.Y = y;
            this.Z = zw.X;
            this.W = zw.Y;
        }

        public Byte4(Byte2 xy, Byte2 zw)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = zw.X;
            this.W = zw.Y;
        }

        public Byte4(byte xyzw)
        {
            this.X = xyzw;
            this.Y = xyzw;
            this.Z = xyzw;
            this.W = xyzw;
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
            return (((((this.X.ToString() + ", ") + this.Y.ToString()) + ", ") + this.Z.ToString()) + ", ") + this.W.ToString();
        }

        public byte this[int index]
        {
            get
            {
                if (index == 0)
                    return this.X;
                else if (index == 1)
                    return this.Y;
                else if (index == 2)
                    return this.Z;
                else if (index == 3)
                    return this.W;

                throw new global::System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    this.X = value;
                else if (index == 1)
                    this.Y = value;
                else if (index == 2)
                    this.Z = value;
                else if (index == 3)
                    this.W = value;
                else
                    throw new global::System.IndexOutOfRangeException();
            }
        }

        public static Int4 operator +(Byte4 a, Byte4 b)
        {
            return new Int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Int4 operator -(Byte4 a, Byte4 b)
        {
            return new Int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Int4 operator *(Byte4 a, Byte4 b)
        {
            return new Int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

        public static Int4 operator /(Byte4 a, Byte4 b)
        {
            return new Int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }

        public static Int4 operator +(Byte4 a, byte b)
        {
            return new Int4(a.X + b, a.Y + b, a.Z + b, a.W + b);
        }

        public static Int4 operator -(Byte4 a, byte b)
        {
            return new Int4(a.X - b, a.Y - b, a.Z - b, a.W - b);
        }

        public static Int4 operator *(Byte4 a, byte b)
        {
            return new Int4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Int4 operator /(Byte4 a, byte b)
        {
            return new Int4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }

        public static bool operator ==(Byte4 a, Byte4 b)
        {
            return Generic.Equals<Byte4>(a, b);
        }

        public static bool operator !=(Byte4 a, Byte4 b)
        {
            return !Generic.Equals<Byte4>(a, b);
        }

        public static explicit operator Byte4(SByte4 v)
        {
            return new Byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W);
        }

        public static explicit operator Byte4(Short4 v)
        {
            return new Byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W);
        }

        public static explicit operator Byte4(UShort4 v)
        {
            return new Byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W);
        }

        public static explicit operator Byte4(Int4 v)
        {
            return new Byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W);
        }

        public static explicit operator Byte4(Float4 v)
        {
            return new Byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W);
        }
    }
}
