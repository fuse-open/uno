using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 4-component vector of 32-bit signed integer values. */
    public intrinsic struct Int4
    {
        public int X, Y, Z, W;
        swizzler int2, int3, int4;

        public int this[int index]
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

        public Int4(int x, int y, int z, int w) { X = x; Y = y; Z = z; W = w; }
        public Int4(int2 xy, int z, int w) { X = xy.X; Y = xy.Y; Z = z; W = w; }
        public Int4(int x, int2 yz, int w) { X = x; Y = yz.X; Z = yz.Y; W = w; }
        public Int4(int x, int y, int2 zw) { X = x; Y = y; Z = zw.X; W = zw.Y; }
        public Int4(int2 xy, int2 zw) { X = xy.X; Y = xy.Y; Z = zw.X; W = zw.Y; }
        public Int4(int3 xyz, int w) { X = xyz.X; Y = xyz.Y; Z = xyz.Z; W = w; }
        public Int4(int x, int3 yzw) { X = x; Y = yzw.X; Z = yzw.Y; W = yzw.Z; }
        public Int4(int xyzw) { X = xyzw; Y = xyzw; Z = xyzw; W = xyzw; }

        public static int4 operator + (int4 a, int4 b) { return int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W); }
        public static int4 operator - (int4 a, int4 b) { return int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W); }
        public static int4 operator * (int4 a, int4 b) { return int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W); }
        public static int4 operator / (int4 a, int4 b) { return int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W); }

        public static int4 operator + (int4 a, int b) { return int4(a.X + b, a.Y + b, a.Z + b, a.W + b); }
        public static int4 operator - (int4 a, int b) { return int4(a.X - b, a.Y - b, a.Z - b, a.W - b); }
        public static int4 operator * (int4 a, int b) { return int4(a.X * b, a.Y * b, a.Z * b, a.W * b); }
        public static int4 operator / (int4 a, int b) { return int4(a.X / b, a.Y / b, a.Z / b, a.W / b); }

        public static bool operator == (int4 a, int4 b) { return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W; }
        public static bool operator != (int4 a, int4 b) { return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W; }

        public static implicit operator int4(byte4 v) { return int4(v.X, v.Y, v.Z, v.W); }
        public static implicit operator int4(sbyte4 v) { return int4(v.X, v.Y, v.Z, v.W); }
        public static implicit operator int4(ushort4 v) { return int4(v.X, v.Y, v.Z, v.W); }
        public static implicit operator int4(short4 v) { return int4(v.X, v.Y, v.Z, v.W); }
        public static explicit operator int4(float4 v) { return int4((int)v.X, (int)v.Y, (int)v.Z, (int)v.W); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + W.ToString(); }
    }
}
