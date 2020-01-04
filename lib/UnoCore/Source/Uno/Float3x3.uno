using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 3x3 matrix of single-precision floating point numbers. */
    public intrinsic struct Float3x3
    {
        public static float3x3 Identity
        {
            get { return float3x3(1.0f,0.0f,0.0f, 0.0f,1.0f,0.0f, 0.0f,0.0f,1.0f); }
        }

        public float M11, M12, M13;
        public float M21, M22, M23;
        public float M31, M32, M33;

        swizzler float2, float3, float4, float2x2, float3x3, float4x4;

        public float3 this[int index]
        {
            get
            {
                if (index == 0) return float3(M11, M12, M13);
                else if (index == 1) return float3(M21, M22, M23);
                else if (index == 2) return float3(M31, M32, M33);
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 0) { M11 = value.X; M12 = value.Y; M13 = value.Z; }
                else if (index == 1) { M21 = value.X; M22 = value.Y; M23 = value.Z; }
                else if (index == 2) { M31 = value.X; M32 = value.Y; M33 = value.Z; }
                else throw new IndexOutOfRangeException();
            }
        }

        public Float3x3(
            float m11, float m12, float m13,
            float m21, float m22, float m23,
            float m31, float m32, float m33)
        {
            M11 = m11; M12 = m12; M13 = m13;
            M21 = m21; M22 = m22; M23 = m23;
            M31 = m31; M32 = m32; M33 = m33;
        }

        public Float3x3(float3 a, float3 b, float3 c)
        {
            M11 = a.X; M12 = a.Y; M13 = a.Z;
            M21 = b.X; M22 = b.Y; M23 = b.Z;
            M31 = c.X; M32 = c.Y; M33 = c.Z;
        }

        public static float3x3 operator - (float3x3 left, float3x3 right)
        {
            float3x3 result;
            result.M11 = left.M11 - right.M11;
            result.M12 = left.M12 - right.M12;
            result.M13 = left.M13 - right.M13;
            result.M21 = left.M21 - right.M21;
            result.M22 = left.M22 - right.M22;
            result.M23 = left.M23 - right.M23;
            result.M31 = left.M31 - right.M31;
            result.M32 = left.M32 - right.M32;
            result.M33 = left.M33 - right.M33;
            return result;
        }

        public static float3x3 operator + (float3x3 left, float3x3 right)
        {
            float3x3 result;
            result.M11 = left.M11 + right.M11;
            result.M12 = left.M12 + right.M12;
            result.M13 = left.M13 + right.M13;
            result.M21 = left.M21 + right.M21;
            result.M22 = left.M22 + right.M22;
            result.M23 = left.M23 + right.M23;
            result.M31 = left.M31 + right.M31;
            result.M32 = left.M32 + right.M32;
            result.M33 = left.M33 + right.M33;
            return result;
        }

        public static float3x3 operator * (float3x3 left, float right)
        {
            float3x3 result;
            result.M11 = left.M11 * right;
            result.M12 = left.M12 * right;
            result.M13 = left.M13 * right;
            result.M21 = left.M21 * right;
            result.M22 = left.M22 * right;
            result.M23 = left.M23 * right;
            result.M31 = left.M31 * right;
            result.M32 = left.M32 * right;
            result.M33 = left.M33 * right;
            return result;
        }

        public static float3x3 operator / (float3x3 left, float right)
        {
            float3x3 result;
            result.M11 = left.M11 / right;
            result.M12 = left.M12 / right;
            result.M13 = left.M13 / right;
            result.M21 = left.M21 / right;
            result.M22 = left.M22 / right;
            result.M23 = left.M23 / right;
            result.M31 = left.M31 / right;
            result.M32 = left.M32 / right;
            result.M33 = left.M33 / right;
            return result;
        }

        public static bool operator == (float3x3 a, float3x3 b)
        {
            return Generic.Equals(a, b);
        }

        public static bool operator != (float3x3 a, float3x3 b)
        {
            return !Generic.Equals(a, b);
        }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return M11.ToString() + "," + M12.ToString() + M13.ToString() + "," +
                   M21.ToString() + "," + M22.ToString() + M23.ToString() + "," +
                   M31.ToString() + "," + M32.ToString() + M33.ToString();
        }
    }
}
