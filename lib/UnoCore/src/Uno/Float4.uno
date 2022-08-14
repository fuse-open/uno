using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 4-component vector of single-precision floating point numbers. */
    public intrinsic struct Float4
    {
        public static float4 Identity
        {
            get { return float4(0.0f, 0.0f, 0.0f, 1.0f); }
        }

        public float X, Y, Z, W;
        swizzler float2, float3, float4;

        public float this[int index]
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

        public Float4(float xyzw) { X = Y = Z = W = xyzw; }
        public Float4(float x, float y, float z, float w) { X = x; Y = y; Z = z; W = w; }
        public Float4(float2 xy, float z, float w) { X = xy.X; Y = xy.Y; Z = z; W = w; }
        public Float4(float x, float2 yz, float w) { X = x; Y = yz.X; Z = yz.Y; W = w; }
        public Float4(float x, float y, float2 zw) { X = x; Y = y; Z = zw.X; W = zw.Y; }
        public Float4(float2 xy, float2 zw) { X = xy.X; Y = xy.Y; Z = zw.X; W = zw.Y; }
        public Float4(float3 xyz, float w) { X = xyz.X; Y = xyz.Y; Z = xyz.Z; W = w; }
        public Float4(float x, float3 yzw) { X = x; Y = yzw.X; Z = yzw.Y; W = yzw.Z; }


        public static float4 operator + (float4 a, float4 b) { return float4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W); }
        public static float4 operator - (float4 a, float4 b) { return float4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W); }
        public static float4 operator * (float4 a, float4 b) { return float4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W); }
        public static float4 operator / (float4 a, float4 b) { return float4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W); }

        public static float4 operator + (float4 a, float b) { return float4(a.X + b, a.Y + b, a.Z + b, a.W + b); }
        public static float4 operator - (float4 a, float b) { return float4(a.X - b, a.Y - b, a.Z - b, a.W - b); }
        public static float4 operator * (float4 a, float b) { return float4(a.X * b, a.Y * b, a.Z * b, a.W * b); }
        public static float4 operator / (float4 a, float b) { return float4(a.X / b, a.Y / b, a.Z / b, a.W / b); }

        public static float4 operator + (float a, float4 b) { return float4(a + b.X, a + b.Y, a + b.Z, a + b.W); }
        public static float4 operator - (float a, float4 b) { return float4(a - b.X, a - b.Y, a - b.Z, a - b.W); }
        public static float4 operator * (float a, float4 b) { return float4(a * b.X, a * b.Y, a * b.Z, a * b.W); }
        public static float4 operator / (float a, float4 b) { return float4(a / b.X, a / b.Y, a / b.Z, a / b.W); }

        public static bool operator == (float4 a, float4 b) { return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W; }
        public static bool operator != (float4 a, float4 b) { return a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W; }

        public static implicit operator float4(byte4 a) { return float4(a.X, a.Y, a.Z, a.W); }
        public static implicit operator float4(sbyte4 a) { return float4(a.X, a.Y, a.Z, a.W); }
        public static implicit operator float4(ushort4 a) { return float4(a.X, a.Y, a.Z, a.W); }
        public static implicit operator float4(short4 a) { return float4(a.X, a.Y, a.Z, a.W); }
        public static implicit operator float4(int4 a) { return float4(a.X, a.Y, a.Z, a.W); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + W.ToString(); }
    }
}
