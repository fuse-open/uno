// This file was generated based on Library/Core/UnoCore/Source/Uno/Short4.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct Short4
    {
        public short X;
        public short Y;
        public short Z;
        public short W;

        public Short4(short xyzw)
        {
            this.X = xyzw;
            this.Y = xyzw;
            this.Z = xyzw;
            this.W = xyzw;
        }

        public Short4(short x, short y, short z, short w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Short4(Short2 xy, short z, short w)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
            this.W = w;
        }

        public Short4(short x, Short2 yz, short w)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
            this.W = w;
        }

        public Short4(short x, short y, Short2 zw)
        {
            this.X = x;
            this.Y = y;
            this.Z = zw.X;
            this.W = zw.Y;
        }

        public Short4(Short2 xy, Short2 zw)
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

        public short this[int index]
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

        public static Int4 operator +(Short4 a, Short4 b)
        {
            return new Int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Int4 operator -(Short4 a, Short4 b)
        {
            return new Int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Int4 operator *(Short4 a, Short4 b)
        {
            return new Int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

        public static Int4 operator /(Short4 a, Short4 b)
        {
            return new Int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }

        public static Int4 operator +(Short4 a, short b)
        {
            return new Int4(a.X + b, a.Y + b, a.Z + b, a.W + b);
        }

        public static Int4 operator -(Short4 a, short b)
        {
            return new Int4(a.X - b, a.Y - b, a.Z - b, a.W - b);
        }

        public static Int4 operator *(Short4 a, short b)
        {
            return new Int4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Int4 operator /(Short4 a, short b)
        {
            return new Int4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }

        public static bool operator ==(Short4 a, Short4 b)
        {
            return (((a.X == b.X) && (a.Y == b.Y)) && (a.Z == b.Z)) && (a.W == b.W);
        }

        public static bool operator !=(Short4 a, Short4 b)
        {
            return (((a.X != b.X) || (a.Y != b.Y)) || (a.Z != b.Z)) || (a.W != b.W);
        }

        public static implicit operator Short4(SByte4 v)
        {
            return new Short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W);
        }

        public static implicit operator Short4(Byte4 v)
        {
            return new Short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W);
        }

        public static explicit operator Short4(UShort4 v)
        {
            return new Short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W);
        }

        public static explicit operator Short4(Int4 v)
        {
            return new Short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W);
        }

        public static explicit operator Short4(Float4 v)
        {
            return new Short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W);
        }
    }
}
