using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 4-component vector of 16-bit signed integers. */
    public intrinsic struct Short4
    {
        public short X, Y, Z, W;
        swizzler short2, short4;

        public short this[int index]
        {
            get
            {
                if (index == 0) return X;
                else if (index == 1) return Y;
                else if (index == 2) return Z;
                else if (index == 3) return W;
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 0) X = value;
                else if (index == 1) Y = value;
                else if (index == 2) Z = value;
                else if (index == 3) W = value;
                else throw new IndexOutOfRangeException();
            }
        }

        public Short4(short xyzw) { X = xyzw; Y = xyzw; Z = xyzw; W = xyzw; }
        public Short4(short x, short y, short z, short w) { X = x; Y = y; Z = z; W = w; }
        public Short4(short2 xy, short z, short w) { X = xy.X; Y = xy.Y; Z = z; W = w; }
        public Short4(short x, short2 yz, short w) { X = x; Y = yz.X; Z = yz.Y; W = w; }
        public Short4(short x, short y, short2 zw) { X = x; Y = y; Z = zw.X; W = zw.Y; }
        public Short4(short2 xy, short2 zw) { X = xy.X; Y = xy.Y; Z = zw.X; W = zw.Y; }

        public static int4 operator + (short4 a, short4 b) { return int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W); }
        public static int4 operator - (short4 a, short4 b) { return int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W); }
        public static int4 operator * (short4 a, short4 b) { return int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W); }
        public static int4 operator / (short4 a, short4 b) { return int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W); }

        public static int4 operator + (short4 a, short b) { return int4(a.X + b, a.Y + b, a.Z + b, a.W + b); }
        public static int4 operator - (short4 a, short b) { return int4(a.X - b, a.Y - b, a.Z - b, a.W - b); }
        public static int4 operator * (short4 a, short b) { return int4(a.X * b, a.Y * b, a.Z * b, a.W * b); }
        public static int4 operator / (short4 a, short b) { return int4(a.X / b, a.Y / b, a.Z / b, a.W / b); }

        public static bool operator == (short4 a, short4 b) { return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W; }
        public static bool operator != (short4 a, short4 b) { return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W; }

        public static implicit operator short4(sbyte4 v) { return short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W); }
        public static implicit operator short4(byte4 v) { return short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W); }
        public static explicit operator short4(ushort4 v) { return short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W); }
        public static explicit operator short4(int4 v) { return short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W); }
        public static explicit operator short4(float4 v) { return short4((short)v.X, (short)v.Y, (short)v.Z, (short)v.W); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + W.ToString(); }
    }
}
