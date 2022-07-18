using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    public intrinsic struct Byte2
    {
        public byte X, Y;
        swizzler byte2, byte4;

        public byte this[int index]
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

        public Byte2(byte xy) { X = xy; Y = xy; }
        public Byte2(byte x, byte y) { X = x; Y = y; }

        public static int2 operator + (byte2 a, byte2 b) { return int2(a.X + b.X, a.Y + b.Y); }
        public static int2 operator - (byte2 a, byte2 b) { return int2(a.X - b.X, a.Y - b.Y); }
        public static int2 operator * (byte2 a, byte2 b) { return int2(a.X * b.X, a.Y * b.Y); }
        public static int2 operator / (byte2 a, byte2 b) { return int2(a.X / b.X, a.Y / b.Y); }

        public static int2 operator + (byte2 a, byte b) { return int2(a.X + b, a.Y + b); }
        public static int2 operator - (byte2 a, byte b) { return int2(a.X - b, a.Y - b); }
        public static int2 operator * (byte2 a, byte b) { return int2(a.X * b, a.Y * b); }
        public static int2 operator / (byte2 a, byte b) { return int2(a.X / b, a.Y / b); }

        public static bool operator == (byte2 a, byte2 b) { return a.X == b.X && a.Y == b.Y; }
        public static bool operator != (byte2 a, byte2 b) { return a.X != b.X || a.Y != b.Y; }

        public static explicit operator byte2(sbyte2 v) { return byte2((byte)v.X, (byte)v.Y); }
        public static explicit operator byte2(short2 v) { return byte2((byte)v.X, (byte)v.Y); }
        public static explicit operator byte2(ushort2 v) { return byte2((byte)v.X, (byte)v.Y); }
        public static explicit operator byte2(int2 v) { return byte2((byte)v.X, (byte)v.Y); }
        public static explicit operator byte2(float2 v) { return byte2((byte)v.X, (byte)v.Y); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString(); }
    }
}
