// This file was generated based on lib/UnoCore/Source/Uno/SByte4.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct SByte4
    {
        public sbyte X;
        public sbyte Y;
        public sbyte Z;
        public sbyte W;

        public SByte4(sbyte xyzw)
        {
            this.X = this.Y = this.Z = this.W = xyzw;
        }

        public SByte4(sbyte x, sbyte y, sbyte z, sbyte w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public SByte4(SByte2 xy, sbyte z, sbyte w)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
            this.W = w;
        }

        public SByte4(sbyte x, SByte2 yz, sbyte w)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
            this.W = w;
        }

        public SByte4(sbyte x, sbyte y, SByte2 zw)
        {
            this.X = x;
            this.Y = y;
            this.Z = zw.X;
            this.W = zw.Y;
        }

        public SByte4(SByte2 xy, SByte2 zw)
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

        public sbyte this[int index]
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

        public static Int4 operator +(SByte4 a, SByte4 b)
        {
            return new Int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Int4 operator -(SByte4 a, SByte4 b)
        {
            return new Int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Int4 operator *(SByte4 a, SByte4 b)
        {
            return new Int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

        public static Int4 operator /(SByte4 a, SByte4 b)
        {
            return new Int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }

        public static Int4 operator +(SByte4 a, sbyte b)
        {
            return new Int4(a.X + b, a.Y + b, a.Z + b, a.W + b);
        }

        public static Int4 operator -(SByte4 a, sbyte b)
        {
            return new Int4(a.X - b, a.Y - b, a.Z - b, a.W - b);
        }

        public static Int4 operator *(SByte4 a, sbyte b)
        {
            return new Int4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Int4 operator /(SByte4 a, sbyte b)
        {
            return new Int4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }

        public static bool operator ==(SByte4 a, SByte4 b)
        {
            return (((a.X == b.X) && (a.Y == b.Y)) && (a.Z == b.Z)) && (a.W == b.W);
        }

        public static bool operator !=(SByte4 a, SByte4 b)
        {
            return (((a.X != b.X) || (a.Y != b.Y)) || (a.Z != b.Z)) || (a.W != b.W);
        }

        public static explicit operator SByte4(Byte4 v)
        {
            return new SByte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W);
        }

        public static explicit operator SByte4(Short4 v)
        {
            return new SByte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W);
        }

        public static explicit operator SByte4(UShort4 v)
        {
            return new SByte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W);
        }

        public static explicit operator SByte4(Int4 v)
        {
            return new SByte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W);
        }

        public static explicit operator SByte4(Float4 v)
        {
            return new SByte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W);
        }
    }
}
