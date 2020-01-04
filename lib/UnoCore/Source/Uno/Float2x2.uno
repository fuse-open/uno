using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    /** 2x2 matrix of single-precision floating point numbers. */
    public intrinsic struct Float2x2
    {
        public static float2x2 Identity
        {
            get { return float2x2(1.0f,0.0f, 0.0f,1.0f); }
        }

        public float M11, M12;
        public float M21, M22;

        swizzler float2, float3, float4, float2x2, float3x3, float4x4;

        public float2 this[int index]
        {
            get
            {
                if (index == 0) return float2(M11, M12);
                else if (index == 1) return float2(M21, M22);
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 0) { M11 = value.X; M12 = value.Y; }
                else if (index == 1) { M21 = value.X; M22 = value.Y; }
                else throw new IndexOutOfRangeException();
            }
        }

        public Float2x2(
            float m11, float m12,
            float m21, float m22)
        {
            M11 = m11; M12 = m12;
            M21 = m21; M22 = m22;
        }

        public Float2x2(float2 a, float2 b)
        {
            M11 = a.X; M12 = a.Y;
            M21 = b.X; M22 = b.Y;
        }

        public static float2x2 operator - (float2x2 left, float2x2 right)
        {
            float2x2 result;
            result.M11 = left.M11 - right.M11;
            result.M12 = left.M12 - right.M12;
            result.M21 = left.M21 - right.M21;
            result.M22 = left.M22 - right.M22;
            return result;
        }

        public static float2x2 operator + (float2x2 left, float2x2 right)
        {
            float2x2 result;
            result.M11 = left.M11 + right.M11;
            result.M12 = left.M12 + right.M12;
            result.M21 = left.M21 + right.M21;
            result.M22 = left.M22 + right.M22;
            return result;
        }

        public static float2x2 operator * (float2x2 left, float right)
        {
            float2x2 result;
            result.M11 = left.M11 * right;
            result.M12 = left.M12 * right;
            result.M21 = left.M21 * right;
            result.M22 = left.M22 * right;
            return result;
        }

        public static float2x2 operator / (float2x2 left, float right)
        {
            float2x2 result;
            result.M11 = left.M11 / right;
            result.M12 = left.M12 / right;
            result.M21 = left.M21 / right;
            result.M22 = left.M22 / right;
            return result;
        }

        public static bool operator == (float2x2 a, float2x2 b)
        {
            return Generic.Equals(a, b);
        }

        public static bool operator != (float2x2 a, float2x2 b)
        {
            return !Generic.Equals(a, b);
        }

        public override bool Equals(object o) { return base.Equals(o); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString()
        {
            return M11.ToString() + "," + M12.ToString() + "," +
                   M21.ToString() + "," + M22.ToString();
        }

    }
}
