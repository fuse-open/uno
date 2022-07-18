using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 2-component vector of single-precision floating point numbers. */
    public intrinsic struct Float2
    {
        public float X, Y;
        swizzler float2, float3, float4;

        public float this[int index]
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

        public Float2(float xy) { X = Y = xy; }
        public Float2(float x, float y) { X = x; Y = y; }

        public static Float2 operator -(Float2 a) { return float2(-a.X, -a.Y); }

        public static float2 operator + (float2 a, float2 b) { return float2(a.X + b.X, a.Y + b.Y); }
        public static float2 operator - (float2 a, float2 b) { return float2(a.X - b.X, a.Y - b.Y); }
        public static float2 operator * (float2 a, float2 b) { return float2(a.X * b.X, a.Y * b.Y); }
        public static float2 operator / (float2 a, float2 b) { return float2(a.X / b.X, a.Y / b.Y); }

        public static float2 operator + (float2 a, float b) { return float2(a.X + b, a.Y + b); }
        public static float2 operator - (float2 a, float b) { return float2(a.X - b, a.Y - b); }
        public static float2 operator * (float2 a, float b) { return float2(a.X * b, a.Y * b); }
        public static float2 operator / (float2 a, float b) { return float2(a.X / b, a.Y / b); }

        public static float2 operator + (float a, float2 b) { return float2(a + b.X, a + b.Y); }
        public static float2 operator - (float a, float2 b) { return float2(a - b.X, a - b.Y); }
        public static float2 operator * (float a, float2 b) { return float2(a * b.X, a * b.Y); }
        public static float2 operator / (float a, float2 b) { return float2(a / b.X, a / b.Y); }

        public static bool operator == (float2 a, float2 b) { return a.X == b.X && a.Y == b.Y; }
        public static bool operator != (float2 a, float2 b) { return a.X != b.X || a.Y != b.Y; }

        public float Ratio { get { return X / Y; } }

        public static implicit operator float2(byte2 a) { return float2(a.X, a.Y); }
        public static implicit operator float2(sbyte2 a) { return float2(a.X, a.Y); }
        public static implicit operator float2(ushort2 a) { return float2(a.X, a.Y); }
        public static implicit operator float2(short2 a) { return float2(a.X, a.Y); }
        public static implicit operator float2(int2 a) { return float2(a.X, a.Y); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString(); }
    }
}
