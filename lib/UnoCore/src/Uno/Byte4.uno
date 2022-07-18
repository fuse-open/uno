using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    public intrinsic struct Byte4
    {
        public byte X, Y, Z, W;
        swizzler byte2, byte4;

        public byte this[int index]
        {
            get
            {
                if (index == 0)
                    return X;
                else if (index == 1)
                    return Y;
                else if (index == 2)
                    return Z;
                else if (index == 3)
                    return W;

                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    X = value;
                else if (index == 1)
                    Y = value;
                else if (index == 2)
                    Z = value;
                else if (index == 3)
                    W = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public Byte4(byte x, byte y, byte z, byte w) { X = x; Y = y; Z = z; W = w; }
        public Byte4(byte2 xy, byte z, byte w) { X = xy.X; Y = xy.Y; Z = z; W = w; }
        public Byte4(byte x, byte2 yz, byte w) { X = x; Y = yz.X; Z = yz.Y; W = w; }
        public Byte4(byte x, byte y, byte2 zw) { X = x; Y = y; Z = zw.X; W = zw.Y; }
        public Byte4(byte2 xy, byte2 zw) { X = xy.X; Y = xy.Y; Z = zw.X; W = zw.Y; }
        public Byte4(byte xyzw) { X = xyzw; Y = xyzw; Z = xyzw; W = xyzw; }

        public static int4 operator + (byte4 a, byte4 b) { return int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W); }
        public static int4 operator - (byte4 a, byte4 b) { return int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W); }
        public static int4 operator * (byte4 a, byte4 b) { return int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W); }
        public static int4 operator / (byte4 a, byte4 b) { return int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W); }

        public static int4 operator + (byte4 a, byte b) { return int4(a.X + b, a.Y + b, a.Z + b, a.W + b); }
        public static int4 operator - (byte4 a, byte b) { return int4(a.X - b, a.Y - b, a.Z - b, a.W - b); }
        public static int4 operator * (byte4 a, byte b) { return int4(a.X * b, a.Y * b, a.Z * b, a.W * b); }
        public static int4 operator / (byte4 a, byte b) { return int4(a.X / b, a.Y / b, a.Z / b, a.W / b); }

        public static bool operator == (byte4 a, byte4 b) { return Generic.Equals(a, b); }
        public static bool operator != (byte4 a, byte4 b) { return !Generic.Equals(a, b); }

        public static explicit operator byte4(sbyte4 v) { return byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W); }
        public static explicit operator byte4(short4 v) { return byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W); }
        public static explicit operator byte4(ushort4 v) { return byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W); }
        public static explicit operator byte4(int4 v) { return byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W); }
        public static explicit operator byte4(float4 v) { return byte4((byte)v.X, (byte)v.Y, (byte)v.Z, (byte)v.W); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + W.ToString(); }
    }
}
