using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 2-component vector of 16-bit signed integers. */
    public intrinsic struct Short2
    {
        public short X, Y;
        swizzler short2, short4;

        public short this[int index]
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
        public Short2(short xy) { X = Y = xy; }
        public Short2(short x, short y) { X = x; Y = y; }

        public static int2 operator + (short2 a, short2 b) { return int2(a.X + b.X, a.Y + b.Y); }
        public static int2 operator - (short2 a, short2 b) { return int2(a.X - b.X, a.Y - b.Y); }
        public static int2 operator * (short2 a, short2 b) { return int2(a.X * b.X, a.Y * b.Y); }
        public static int2 operator / (short2 a, short2 b) { return int2(a.X / b.X, a.Y / b.Y); }

        public static int2 operator + (short2 a, short b) { return int2(a.X + b, a.Y + b); }
        public static int2 operator - (short2 a, short b) { return int2(a.X - b, a.Y - b); }
        public static int2 operator * (short2 a, short b) { return int2(a.X * b, a.Y * b); }
        public static int2 operator / (short2 a, short b) { return int2(a.X / b, a.Y / b); }

        public static bool operator == (short2 a, short2 b) { return a.X == b.X && a.Y == b.Y; }
        public static bool operator != (short2 a, short2 b) { return a.X != b.X || a.Y != b.Y; }

        public static implicit operator short2(sbyte2 v) { return short2((short)v.X, (short)v.Y); }
        public static implicit operator short2(byte2 v) { return short2((short)v.X, (short)v.Y); }
        public static explicit operator short2(ushort2 v) { return short2((short)v.X, (short)v.Y); }
        public static explicit operator short2(int2 v) { return short2((short)v.X, (short)v.Y); }
        public static explicit operator short2(float2 v) { return short2((short)v.X, (short)v.Y); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString(); }
    }
}
