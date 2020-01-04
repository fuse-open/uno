using Uno.Compiler.ExportTargetInterop;
using Uno.Vector;

namespace Uno
{
    /** 4x4 matrix of single-precision floating point numbers. */
    public intrinsic struct Float4x4
    {
        public static float4x4 Identity
        {
            get { return float4x4(1.0f,0.0f,0.0f,0.0f, 0.0f,1.0f,0.0f,0.0f, 0.0f,0.0f,1.0f,0.0f, 0.0f,0.0f,0.0f,1.0f); }
        }

        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;
        public float M41, M42, M43, M44;

        swizzler float2, float3, float4, float2x2, float3x3, float4x4;

        public float4 this[int index]
        {
            get
            {
                if (index == 0) return float4(M11, M12, M13, M14);
                else if (index == 1) return float4(M21, M22, M23, M24);
                else if (index == 2) return float4(M31, M32, M33, M34);
                else if (index == 3) return float4(M41, M42, M43, M44);
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 0) { M11 = value.X; M12 = value.Y; M13 = value.Z; M14 = value.W; }
                else if (index == 1) { M21 = value.X; M22 = value.Y; M23 = value.Z; M24 = value.W; }
                else if (index == 2) { M31 = value.X; M32 = value.Y; M33 = value.Z; M34 = value.W; }
                else if (index == 3) { M41 = value.X; M42 = value.Y; M43 = value.Z; M44 = value.W; }
                else throw new IndexOutOfRangeException();
            }
        }

        public Float4x4(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }

        public Float4x4(float4 a, float4 b, float4 c, float4 d)
        {
            M11 = a.X; M12 = a.Y; M13 = a.Z; M14 = a.W;
            M21 = b.X; M22 = b.Y; M23 = b.Z; M24 = b.W;
            M31 = c.X; M32 = c.Y; M33 = c.Z; M34 = c.W;
            M41 = d.X; M42 = d.Y; M43 = d.Z; M44 = d.W;
        }

        public Float4x4(float3x3 f)
        {
            M11 = f.M11; M12 = f.M12; M13 = f.M13; M14 = 0;
            M21 = f.M21; M22 = f.M22; M23 = f.M23; M24 = 0;
            M31 = f.M31; M32 = f.M32; M33 = f.M33; M34 = 0;
            M41 = 0;     M42 = 0;      M43 = 0;       M44 = 1;
        }

        public static float4x4 operator - (float4x4 left, float4x4 right)
        {
            float4x4 result;
            result.M11 = left.M11 - right.M11;
            result.M12 = left.M12 - right.M12;
            result.M13 = left.M13 - right.M13;
            result.M14 = left.M14 - right.M14;
            result.M21 = left.M21 - right.M21;
            result.M22 = left.M22 - right.M22;
            result.M23 = left.M23 - right.M23;
            result.M24 = left.M24 - right.M24;
            result.M31 = left.M31 - right.M31;
            result.M32 = left.M32 - right.M32;
            result.M33 = left.M33 - right.M33;
            result.M34 = left.M34 - right.M34;
            result.M41 = left.M41 - right.M41;
            result.M42 = left.M42 - right.M42;
            result.M43 = left.M43 - right.M43;
            result.M44 = left.M44 - right.M44;
            return result;
        }

        public static float4x4 operator + (float4x4 left, float4x4 right)
        {
            float4x4 result;
            result.M11 = left.M11 + right.M11;
            result.M12 = left.M12 + right.M12;
            result.M13 = left.M13 + right.M13;
            result.M14 = left.M14 + right.M14;
            result.M21 = left.M21 + right.M21;
            result.M22 = left.M22 + right.M22;
            result.M23 = left.M23 + right.M23;
            result.M24 = left.M24 + right.M24;
            result.M31 = left.M31 + right.M31;
            result.M32 = left.M32 + right.M32;
            result.M33 = left.M33 + right.M33;
            result.M34 = left.M34 + right.M34;
            result.M41 = left.M41 + right.M41;
            result.M42 = left.M42 + right.M42;
            result.M43 = left.M43 + right.M43;
            result.M44 = left.M44 + right.M44;
            return result;
        }

        public static float4x4 operator * (float4x4 left, float right)
        {
            float4x4 result;
            result.M11 = left.M11 * right;
            result.M12 = left.M12 * right;
            result.M13 = left.M13 * right;
            result.M14 = left.M14 * right;
            result.M21 = left.M21 * right;
            result.M22 = left.M22 * right;
            result.M23 = left.M23 * right;
            result.M24 = left.M24 * right;
            result.M31 = left.M31 * right;
            result.M32 = left.M32 * right;
            result.M33 = left.M33 * right;
            result.M34 = left.M34 * right;
            result.M41 = left.M41 * right;
            result.M42 = left.M42 * right;
            result.M43 = left.M43 * right;
            result.M44 = left.M44 * right;
            return result;
        }

        public static float4x4 operator / (float4x4 left, float right)
        {
            float4x4 result;
            result.M11 = left.M11 / right;
            result.M12 = left.M12 / right;
            result.M13 = left.M13 / right;
            result.M14 = left.M14 / right;
            result.M21 = left.M21 / right;
            result.M22 = left.M22 / right;
            result.M23 = left.M23 / right;
            result.M24 = left.M24 / right;
            result.M31 = left.M31 / right;
            result.M32 = left.M32 / right;
            result.M33 = left.M33 / right;
            result.M34 = left.M34 / right;
            result.M41 = left.M41 / right;
            result.M42 = left.M42 / right;
            result.M43 = left.M43 / right;
            result.M44 = left.M44 / right;
            return result;
        }

        public static bool operator == (float4x4 a, float4x4 b)
        {
            return Generic.Equals(a, b);
        }

        public static bool operator != (float4x4 a, float4x4 b)
        {
            return !Generic.Equals(a, b);
        }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return M11.ToString() + "," + M12.ToString() + "," + M13.ToString() + "," + M14.ToString() + "," +
                   M21.ToString() + "," + M22.ToString() + "," + M23.ToString() + "," + M24.ToString() + "," +
                   M31.ToString() + "," + M32.ToString() + "," + M33.ToString() + "," + M34.ToString() + "," +
                   M41.ToString() + "," + M42.ToString() + "," + M43.ToString() + "," + M44.ToString();
        }
    }
}
