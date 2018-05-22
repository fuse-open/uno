// This file was generated based on lib/UnoCore/Source/Uno/Float4x4.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct Float4x4
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public Float4x4(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        public Float4x4(Float4 a, Float4 b, Float4 c, Float4 d)
        {
            this.M11 = a.X;
            this.M12 = a.Y;
            this.M13 = a.Z;
            this.M14 = a.W;
            this.M21 = b.X;
            this.M22 = b.Y;
            this.M23 = b.Z;
            this.M24 = b.W;
            this.M31 = c.X;
            this.M32 = c.Y;
            this.M33 = c.Z;
            this.M34 = c.W;
            this.M41 = d.X;
            this.M42 = d.Y;
            this.M43 = d.Z;
            this.M44 = d.W;
        }

        public Float4x4(Float3x3 f)
        {
            this.M11 = f.M11;
            this.M12 = f.M12;
            this.M13 = f.M13;
            this.M14 = 0.0f;
            this.M21 = f.M21;
            this.M22 = f.M22;
            this.M23 = f.M23;
            this.M24 = 0.0f;
            this.M31 = f.M31;
            this.M32 = f.M32;
            this.M33 = f.M33;
            this.M34 = 0.0f;
            this.M41 = 0.0f;
            this.M42 = 0.0f;
            this.M43 = 0.0f;
            this.M44 = 1.0f;
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return (((((((((((((((((((((((((((((this.M11.ToString() + ",") + this.M12.ToString()) + ",") + this.M13.ToString()) + ",") + this.M14.ToString()) + ",") + this.M21.ToString()) + ",") + this.M22.ToString()) + ",") + this.M23.ToString()) + ",") + this.M24.ToString()) + ",") + this.M31.ToString()) + ",") + this.M32.ToString()) + ",") + this.M33.ToString()) + ",") + this.M34.ToString()) + ",") + this.M41.ToString()) + ",") + this.M42.ToString()) + ",") + this.M43.ToString()) + ",") + this.M44.ToString();
        }

        public static Float4x4 Identity
        {
            get { return new Float4x4(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f); }
        }

        public Float4 this[int index]
        {
            get
            {
                if (index == 0)
                    return new Float4(this.M11, this.M12, this.M13, this.M14);
                else if (index == 1)
                    return new Float4(this.M21, this.M22, this.M23, this.M24);
                else if (index == 2)
                    return new Float4(this.M31, this.M32, this.M33, this.M34);
                else if (index == 3)
                    return new Float4(this.M41, this.M42, this.M43, this.M44);
                else
                    throw new global::System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                {
                    this.M11 = value.X;
                    this.M12 = value.Y;
                    this.M13 = value.Z;
                    this.M14 = value.W;
                }
                else if (index == 1)
                {
                    this.M21 = value.X;
                    this.M22 = value.Y;
                    this.M23 = value.Z;
                    this.M24 = value.W;
                }
                else if (index == 2)
                {
                    this.M31 = value.X;
                    this.M32 = value.Y;
                    this.M33 = value.Z;
                    this.M34 = value.W;
                }
                else if (index == 3)
                {
                    this.M41 = value.X;
                    this.M42 = value.Y;
                    this.M43 = value.Z;
                    this.M44 = value.W;
                }
                else
                    throw new global::System.IndexOutOfRangeException();
            }
        }

        public static Float4x4 operator -(Float4x4 left, Float4x4 right)
        {
            Float4x4 result;
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

        public static Float4x4 operator +(Float4x4 left, Float4x4 right)
        {
            Float4x4 result;
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

        public static Float4x4 operator *(Float4x4 left, float right)
        {
            Float4x4 result;
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

        public static Float4x4 operator /(Float4x4 left, float right)
        {
            Float4x4 result;
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

        public static bool operator ==(Float4x4 a, Float4x4 b)
        {
            return Generic.Equals<Float4x4>(a, b);
        }

        public static bool operator !=(Float4x4 a, Float4x4 b)
        {
            return !Generic.Equals<Float4x4>(a, b);
        }
    }
}
