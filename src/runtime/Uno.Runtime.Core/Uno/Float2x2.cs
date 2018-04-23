// This file was generated based on Library/Core/UnoCore/Source/Uno/Float2x2.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct Float2x2
    {
        public float M11;
        public float M12;
        public float M21;
        public float M22;

        public Float2x2(float m11, float m12, float m21, float m22)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M21 = m21;
            this.M22 = m22;
        }

        public Float2x2(Float2 a, Float2 b)
        {
            this.M11 = a.X;
            this.M12 = a.Y;
            this.M21 = b.X;
            this.M22 = b.Y;
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
            return (((((this.M11.ToString() + ",") + this.M12.ToString()) + ",") + this.M21.ToString()) + ",") + this.M22.ToString();
        }

        public static Float2x2 Identity
        {
            get { return new Float2x2(1.0f, 0.0f, 0.0f, 1.0f); }
        }

        public Float2 this[int index]
        {
            get
            {
                if (index == 0)
                    return new Float2(this.M11, this.M12);
                else if (index == 1)
                    return new Float2(this.M21, this.M22);
                else
                    throw new global::System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                {
                    this.M11 = value.X;
                    this.M12 = value.Y;
                }
                else if (index == 1)
                {
                    this.M21 = value.X;
                    this.M22 = value.Y;
                }
                else
                    throw new global::System.IndexOutOfRangeException();
            }
        }

        public static Float2x2 operator -(Float2x2 left, Float2x2 right)
        {
            Float2x2 result;
            result.M11 = left.M11 - right.M11;
            result.M12 = left.M12 - right.M12;
            result.M21 = left.M21 - right.M21;
            result.M22 = left.M22 - right.M22;
            return result;
        }

        public static Float2x2 operator +(Float2x2 left, Float2x2 right)
        {
            Float2x2 result;
            result.M11 = left.M11 + right.M11;
            result.M12 = left.M12 + right.M12;
            result.M21 = left.M21 + right.M21;
            result.M22 = left.M22 + right.M22;
            return result;
        }

        public static Float2x2 operator *(Float2x2 left, float right)
        {
            Float2x2 result;
            result.M11 = left.M11 * right;
            result.M12 = left.M12 * right;
            result.M21 = left.M21 * right;
            result.M22 = left.M22 * right;
            return result;
        }

        public static Float2x2 operator /(Float2x2 left, float right)
        {
            Float2x2 result;
            result.M11 = left.M11 / right;
            result.M12 = left.M12 / right;
            result.M21 = left.M21 / right;
            result.M22 = left.M22 / right;
            return result;
        }

        public static bool operator ==(Float2x2 a, Float2x2 b)
        {
            return Generic.Equals<Float2x2>(a, b);
        }

        public static bool operator !=(Float2x2 a, Float2x2 b)
        {
            return !Generic.Equals<Float2x2>(a, b);
        }
    }
}
