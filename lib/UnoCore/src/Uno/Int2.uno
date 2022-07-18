using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 2-component vector of 32-bit signed integer values. */
    public intrinsic struct Int2
    {
        public int X, Y;
        swizzler int2, int3, int4;

        public int this[int index]
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

        public Int2(int xy) { X = Y = xy; }
        public Int2(int x, int y) { X = x; Y = y; }

        public static int2 operator + (int2 a, int2 b) { return int2(a.X + b.X, a.Y + b.Y); }
        public static int2 operator - (int2 a, int2 b) { return int2(a.X - b.X, a.Y - b.Y); }
        public static int2 operator * (int2 a, int2 b) { return int2(a.X * b.X, a.Y * b.Y); }
        public static int2 operator / (int2 a, int2 b) { return int2(a.X / b.X, a.Y / b.Y); }

        public static int2 operator + (int2 a, int b) { return int2(a.X + b, a.Y + b); }
        public static int2 operator - (int2 a, int b) { return int2(a.X - b, a.Y - b); }
        public static int2 operator * (int2 a, int b) { return int2(a.X * b, a.Y * b); }
        public static int2 operator / (int2 a, int b) { return int2(a.X / b, a.Y / b); }

        public static bool operator == (int2 a, int2 b) { return a.X == b.X && a.Y == b.Y; }
        public static bool operator != (int2 a, int2 b) { return a.X != b.X || a.Y != b.Y; }

        public float Ratio { get { return (float)X / (float)Y; } }

        public static implicit operator int2(byte2 v) { return int2(v.X, v.Y); }
        public static implicit operator int2(sbyte2 v) { return int2(v.X, v.Y); }
        public static implicit operator int2(ushort2 v) { return int2(v.X, v.Y); }
        public static implicit operator int2(short2 v) { return int2(v.X, v.Y); }
        public static explicit operator int2(float2 v) { return int2((int)v.X, (int)v.Y); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString(); }
    }
}
