// This file was generated based on Library/Core/UnoCore/Source/Uno/Float3x3.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct Float3x3
    {
        public float M11;
        public float M12;
        public float M13;
        public float M21;
        public float M22;
        public float M23;
        public float M31;
        public float M32;
        public float M33;

        public Float3x3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
        }

        public Float3x3(Float3 a, Float3 b, Float3 c)
        {
            this.M11 = a.X;
            this.M12 = a.Y;
            this.M13 = a.Z;
            this.M21 = b.X;
            this.M22 = b.Y;
            this.M23 = b.Z;
            this.M31 = c.X;
            this.M32 = c.Y;
            this.M33 = c.Z;
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
            return ((((((((((((this.M11.ToString() + ",") + this.M12.ToString()) + this.M13.ToString()) + ",") + this.M21.ToString()) + ",") + this.M22.ToString()) + this.M23.ToString()) + ",") + this.M31.ToString()) + ",") + this.M32.ToString()) + this.M33.ToString();
        }

        public static Float3x3 Identity
        {
            get { return new Float3x3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f); }
        }

        public Float3 this[int index]
        {
            get
            {
                if (index == 0)
                    return new Float3(this.M11, this.M12, this.M13);
                else if (index == 1)
                    return new Float3(this.M21, this.M22, this.M23);
                else if (index == 2)
                    return new Float3(this.M31, this.M32, this.M33);
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
                }
                else if (index == 1)
                {
                    this.M21 = value.X;
                    this.M22 = value.Y;
                    this.M23 = value.Z;
                }
                else if (index == 2)
                {
                    this.M31 = value.X;
                    this.M32 = value.Y;
                    this.M33 = value.Z;
                }
                else
                    throw new global::System.IndexOutOfRangeException();
            }
        }

        public static Float3x3 operator -(Float3x3 left, Float3x3 right)
        {
            Float3x3 result;
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

        public static Float3x3 operator +(Float3x3 left, Float3x3 right)
        {
            Float3x3 result;
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

        public static Float3x3 operator *(Float3x3 left, float right)
        {
            Float3x3 result;
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

        public static Float3x3 operator /(Float3x3 left, float right)
        {
            Float3x3 result;
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

        public static bool operator ==(Float3x3 a, Float3x3 b)
        {
            return Generic.Equals<Float3x3>(a, b);
        }

        public static bool operator !=(Float3x3 a, Float3x3 b)
        {
            return !Generic.Equals<Float3x3>(a, b);
        }
    }
}
