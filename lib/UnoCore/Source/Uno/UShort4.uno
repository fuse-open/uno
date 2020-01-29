using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    public intrinsic struct UShort4
    {
        public ushort X, Y, Z, W;
        swizzler ushort2, ushort4;

        public ushort this[int index]
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

        public UShort4(ushort xyzw) { X = xyzw; Y = xyzw; Z = xyzw; W = xyzw; }
        public UShort4(ushort x, ushort y, ushort z, ushort w) { X = x; Y = y; Z = z; W = w; }
        public UShort4(ushort2 xy, ushort z, ushort w) { X = xy.X; Y = xy.Y; Z = z; W = w; }
        public UShort4(ushort x, ushort2 yz, ushort w) { X = x; Y = yz.X; Z = yz.Y; W = w; }
        public UShort4(ushort x, ushort y, ushort2 zw) { X = x; Y = y; Z = zw.X; W = zw.Y; }
        public UShort4(ushort2 xy, ushort2 zw) { X = xy.X; Y = xy.Y; Z = zw.X; W = zw.Y; }

        public static int4 operator + (ushort4 a, ushort4 b) { return int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W); }
        public static int4 operator - (ushort4 a, ushort4 b) { return int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W); }
        public static int4 operator * (ushort4 a, ushort4 b) { return int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W); }
        public static int4 operator / (ushort4 a, ushort4 b) { return int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W); }

        public static int4 operator + (ushort4 a, ushort b) { return int4(a.X + b, a.Y + b, a.Z + b, a.W + b); }
        public static int4 operator - (ushort4 a, ushort b) { return int4(a.X - b, a.Y - b, a.Z - b, a.W - b); }
        public static int4 operator * (ushort4 a, ushort b) { return int4(a.X * b, a.Y * b, a.Z * b, a.W * b); }
        public static int4 operator / (ushort4 a, ushort b) { return int4(a.X / b, a.Y / b, a.Z / b, a.W / b); }

        public static bool operator == (ushort4 a, ushort4 b) { return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W; }
        public static bool operator != (ushort4 a, ushort4 b) { return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W; }

        public static implicit operator ushort4(sbyte4 v) { return ushort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W); }
        public static implicit operator ushort4(byte4 v) { return ushort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W); }
        public static explicit operator ushort4(short4 v) { return ushort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W); }
        public static explicit operator ushort4(int4 v) { return ushort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W); }
        public static explicit operator ushort4(float4 v) { return ushort4((ushort)v.X, (ushort)v.Y, (ushort)v.Z, (ushort)v.W); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + W.ToString(); }
    }
}
