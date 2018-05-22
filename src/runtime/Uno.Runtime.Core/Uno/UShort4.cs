// This file was generated based on lib/UnoCore/Source/Uno/UShort4.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct UShort4
    {
        public ushort X;
        public ushort Y;
        public ushort Z;
        public ushort W;

        public UShort4(ushort xyzw)
        {
            this.X = xyzw;
            this.Y = xyzw;
            this.Z = xyzw;
            this.W = xyzw;
        }

        public UShort4(ushort x, ushort y, ushort z, ushort w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public UShort4(UShort2 xy, ushort z, ushort w)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
            this.W = w;
        }

        public UShort4(ushort x, UShort2 yz, ushort w)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
            this.W = w;
        }

        public UShort4(ushort x, ushort y, UShort2 zw)
        {
            this.X = x;
            this.Y = y;
            this.Z = zw.X;
            this.W = zw.Y;
        }

        public UShort4(UShort2 xy, UShort2 zw)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = zw.X;
            this.W = zw.Y;
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

        public ushort this[int index]
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
                else
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

        public static Int4 operator +(UShort4 a, UShort4 b)
        {
            return new Int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Int4 operator -(UShort4 a, UShort4 b)
        {
            return new Int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Int4 operator *(UShort4 a, UShort4 b)
        {
            return new Int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

        public static Int4 operator /(UShort4 a, UShort4 b)
        {
            return new Int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }

        public static Int4 operator +(UShort4 a, ushort b)
        {
            return new Int4(a.X + b, a.Y + b, a.Z + b, a.W + b);
        }

        public static Int4 operator -(UShort4 a, ushort b)
        {
            return new Int4(a.X - b, a.Y - b, a.Z - b, a.W - b);
        }

        public static Int4 operator *(UShort4 a, ushort b)
        {
            return new Int4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Int4 operator /(UShort4 a, ushort b)
        {
            return new Int4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }

        public static bool operator ==(UShort4 a, UShort4 b)
        {
            return (((a.X == b.X) && (a.Y == b.Y)) && (a.Z == b.Z)) && (a.W == b.W);
        }

        public static bool operator !=(UShort4 a, UShort4 b)
        {
            return (((a.X != b.X) || (a.Y != b.Y)) || (a.Z != b.Z)) || (a.W != b.W);
        }

        public static implicit operator UShort4(SByte4 v)
        {
            return new UShort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W);
        }

        public static implicit operator UShort4(Byte4 v)
        {
            return new UShort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W);
        }

        public static explicit operator UShort4(Short4 v)
        {
            return new UShort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W);
        }

        public static explicit operator UShort4(Int4 v)
        {
            return new UShort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W);
        }

        public static explicit operator UShort4(Float4 v)
        {
            return new UShort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W);
        }
    }
}
