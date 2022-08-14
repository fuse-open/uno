using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 4-component vector of 8-bit signed integers. */
    public intrinsic struct SByte4
    {
        public sbyte X, Y, Z, W;
        swizzler sbyte2, sbyte4;

        public sbyte this[int index]
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

        public SByte4(sbyte xyzw) { X = Y = Z = W = xyzw;}
        public SByte4(sbyte x, sbyte y, sbyte z, sbyte w) { X = x; Y = y; Z = z; W = w; }
        public SByte4(sbyte2 xy, sbyte z, sbyte w) { X = xy.X; Y = xy.Y; Z = z; W = w; }
        public SByte4(sbyte x, sbyte2 yz, sbyte w) { X = x; Y = yz.X; Z = yz.Y; W = w; }
        public SByte4(sbyte x, sbyte y, sbyte2 zw) { X = x; Y = y; Z = zw.X; W = zw.Y; }
        public SByte4(sbyte2 xy, sbyte2 zw) { X = xy.X; Y = xy.Y; Z = zw.X; W = zw.Y; }

        public static int4 operator + (sbyte4 a, sbyte4 b) { return int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W); }
        public static int4 operator - (sbyte4 a, sbyte4 b) { return int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W); }
        public static int4 operator * (sbyte4 a, sbyte4 b) { return int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W); }
        public static int4 operator / (sbyte4 a, sbyte4 b) { return int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W); }

        public static int4 operator + (sbyte4 a, sbyte b) { return int4(a.X + b, a.Y + b, a.Z + b, a.W + b); }
        public static int4 operator - (sbyte4 a, sbyte b) { return int4(a.X - b, a.Y - b, a.Z - b, a.W - b); }
        public static int4 operator * (sbyte4 a, sbyte b) { return int4(a.X * b, a.Y * b, a.Z * b, a.W * b); }
        public static int4 operator / (sbyte4 a, sbyte b) { return int4(a.X / b, a.Y / b, a.Z / b, a.W / b); }

        public static bool operator == (sbyte4 a, sbyte4 b) { return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W; }
        public static bool operator != (sbyte4 a, sbyte4 b) { return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W; }

        public static explicit operator sbyte4(byte4 v) { return sbyte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W); }
        public static explicit operator sbyte4(short4 v) { return sbyte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W); }
        public static explicit operator sbyte4(ushort4 v) { return sbyte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W); }
        public static explicit operator sbyte4(int4 v) { return sbyte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W); }
        public static explicit operator sbyte4(float4 v) { return sbyte4((sbyte)v.X, (sbyte)v.Y, (sbyte)v.Z, (sbyte)v.W); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + W.ToString(); }
    }
}
