using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    public intrinsic struct UShort2
    {
        public ushort X, Y;
        swizzler ushort2, ushort4;

        public ushort this[int index]
        {
            get
            {
                if (index == 0) return X;
                else if (index == 1) return Y;
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 0) X = value;
                else if (index == 1) Y = value;
                else throw new IndexOutOfRangeException();
            }
        }
        public UShort2(ushort xy) { X = Y = xy; }
        public UShort2(ushort x, ushort y) { X = x; Y = y; }

        public static int2 operator + (ushort2 a, ushort2 b) { return int2(a.X + b.X, a.Y + b.Y); }
        public static int2 operator - (ushort2 a, ushort2 b) { return int2(a.X - b.X, a.Y - b.Y); }
        public static int2 operator * (ushort2 a, ushort2 b) { return int2(a.X * b.X, a.Y * b.Y); }
        public static int2 operator / (ushort2 a, ushort2 b) { return int2(a.X / b.X, a.Y / b.Y); }

        public static int2 operator + (ushort2 a, ushort b) { return int2(a.X + b, a.Y + b); }
        public static int2 operator - (ushort2 a, ushort b) { return int2(a.X - b, a.Y - b); }
        public static int2 operator * (ushort2 a, ushort b) { return int2(a.X * b, a.Y * b); }
        public static int2 operator / (ushort2 a, ushort b) { return int2(a.X / b, a.Y / b); }

        public static bool operator == (ushort2 a, ushort2 b) { return a.X == b.X && a.Y == b.Y; }
        public static bool operator != (ushort2 a, ushort2 b) { return a.X != b.X || a.Y != b.Y; }

        public static implicit operator ushort2(sbyte2 v) { return ushort2((ushort)v.X, (ushort)v.Y); }
        public static implicit operator ushort2(byte2 v) { return ushort2((ushort)v.X, (ushort)v.Y); }
        public static explicit operator ushort2(short2 v) { return ushort2((ushort)v.X, (ushort)v.Y); }
        public static explicit operator ushort2(int2 v) { return ushort2((ushort)v.X, (ushort)v.Y); }
        public static explicit operator ushort2(float2 v) { return ushort2((ushort)v.X, (ushort)v.Y); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString(); }
    }
}
