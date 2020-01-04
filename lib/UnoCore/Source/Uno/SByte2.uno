using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 2-component vector of 8-bit signed integers. */
    public intrinsic struct SByte2
    {
        public sbyte X, Y;
        swizzler sbyte2, sbyte4;

        public sbyte this[int index]
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

        public SByte2(sbyte x, sbyte y) { X = x; Y = y; }
        public SByte2(sbyte xy) { X = Y = xy; }

        public static int2 operator + (sbyte2 a, sbyte2 b) { return int2(a.X + b.X, a.Y + b.Y); }
        public static int2 operator - (sbyte2 a, sbyte2 b) { return int2(a.X - b.X, a.Y - b.Y); }
        public static int2 operator * (sbyte2 a, sbyte2 b) { return int2(a.X * b.X, a.Y * b.Y); }
        public static int2 operator / (sbyte2 a, sbyte2 b) { return int2(a.X / b.X, a.Y / b.Y); }

        public static int2 operator + (sbyte2 a, sbyte b) { return int2(a.X + b, a.Y + b); }
        public static int2 operator - (sbyte2 a, sbyte b) { return int2(a.X - b, a.Y - b); }
        public static int2 operator * (sbyte2 a, sbyte b) { return int2(a.X * b, a.Y * b); }
        public static int2 operator / (sbyte2 a, sbyte b) { return int2(a.X / b, a.Y / b); }

        public static bool operator == (sbyte2 a, sbyte2 b) { return a.X == b.X && a.Y == b.Y; }
        public static bool operator != (sbyte2 a, sbyte2 b) { return a.X != b.X || a.Y != b.Y; }

        public static explicit operator sbyte2(byte2 v) { return sbyte2((sbyte)v.X, (sbyte)v.Y); }
        public static explicit operator sbyte2(short2 v) { return sbyte2((sbyte)v.X, (sbyte)v.Y); }
        public static explicit operator sbyte2(ushort2 v) { return sbyte2((sbyte)v.X, (sbyte)v.Y); }
        public static explicit operator sbyte2(int2 v) { return sbyte2((sbyte)v.X, (sbyte)v.Y); }
        public static explicit operator sbyte2(float2 v) { return sbyte2((sbyte)v.X, (sbyte)v.Y); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString(); }
    }
}
