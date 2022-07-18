using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 3-component vector of single-precision floating point numbers. */
    public intrinsic struct Float3
    {
        public float X, Y, Z;
        swizzler float2, float3, float4;

        public float this[int index]
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

        public Float3(float xyz) { X = Y = Z = xyz; }
        public Float3(float x, float y, float z) { X = x; Y = y; Z = z; }
        public Float3(float2 xy, float z) { X = xy.X; Y = xy.Y; Z = z; }
        public Float3(float x, float2 yz) { X = x; Y = yz.X; Z = yz.Y; }

        public static float3 operator + (float3 a, float3 b) { return float3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static float3 operator - (float3 a, float3 b) { return float3(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
        public static float3 operator * (float3 a, float3 b) { return float3(a.X * b.X, a.Y * b.Y, a.Z * b.Z); }
        public static float3 operator / (float3 a, float3 b) { return float3(a.X / b.X, a.Y / b.Y, a.Z / b.Z); }

        public static float3 operator + (float3 a, float b) { return float3(a.X + b, a.Y + b, a.Z + b); }
        public static float3 operator - (float3 a, float b) { return float3(a.X - b, a.Y - b, a.Z - b); }
        public static float3 operator * (float3 a, float b) { return float3(a.X * b, a.Y * b, a.Z * b); }
        public static float3 operator / (float3 a, float b) { return float3(a.X / b, a.Y / b, a.Z / b); }

        public static float3 operator + (float a, float3 b) { return float3(a + b.X, a + b.Y, a + b.Z); }
        public static float3 operator - (float a, float3 b) { return float3(a - b.X, a - b.Y, a - b.Z); }
        public static float3 operator * (float a, float3 b) { return float3(a * b.X, a * b.Y, a * b.Z); }
        public static float3 operator / (float a, float3 b) { return float3(a / b.X, a / b.Y, a / b.Z); }

        public static bool operator == (float3 a, float3 b) { return a.X == b.X && a.Y == b.Y && a.Z == b.Z; }
        public static bool operator != (float3 a, float3 b) { return a.X != b.X || a.Y != b.Y || a.Z != b.Z; }

        public static float3 operator - (float3 a) { return a * -1.0f; }

        public static implicit operator float3(int3 a) { return float3(a.X, a.Y, a.Z); }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString(); }
    }
}
