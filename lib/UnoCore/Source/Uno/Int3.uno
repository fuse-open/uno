using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 3-component vector of 32-bit signed integer values. */
    public intrinsic struct Int3
    {
        public int X, Y, Z;
        swizzler int2, int3, int4;

        public int this[int index]
        {
            get
            {
                if (index == 0) return X;
                else if (index == 1) return Y;
                else if (index == 2) return Z;
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 0) X = value;
                else if (index == 1) Y = value;
                else if (index == 2) Z = value;
                else throw new IndexOutOfRangeException();
            }
        }

        public Int3(int x, int y, int z) { X = x; Y = y; Z = z; }
        public Int3(int2 xy, int z) { X = xy.X; Y = xy.Y; Z = z; }
        public Int3(int x, int2 yz) { X = x; Y = yz.X; Z = yz.Y; }
        public Int3(int xyz) { X = xyz; Y = xyz; Z = xyz; }

        public static int3 operator + (int3 a, int3 b) { return int3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static int3 operator - (int3 a, int3 b) { return int3(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
        public static int3 operator * (int3 a, int3 b) { return int3(a.X * b.X, a.Y * b.Y, a.Z * b.Z); }
        public static int3 operator / (int3 a, int3 b) { return int3(a.X / b.X, a.Y / b.Y, a.Z / b.Z); }

        public static int3 operator + (int3 a, int b) { return int3(a.X + b, a.Y + b, a.Z + b); }
        public static int3 operator - (int3 a, int b) { return int3(a.X - b, a.Y - b, a.Z - b); }
        public static int3 operator * (int3 a, int b) { return int3(a.X * b, a.Y * b, a.Z * b); }
        public static int3 operator / (int3 a, int b) { return int3(a.X / b, a.Y / b, a.Z / b); }

        public static bool operator == (int3 a, int3 b) { return a.X == b.X && a.Y == b.Y && a.Z == b.Z; }
        public static bool operator != (int3 a, int3 b) { return a.X != b.X || a.Y != b.Y || a.Z != b.Z; }

        public override string ToString() { return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString(); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public static explicit operator int3(float3 v) { return int3((int)v.X, (int)v.Y, (int)v.Z); }
    }
}
