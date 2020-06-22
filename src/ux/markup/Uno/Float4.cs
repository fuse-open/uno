// This file was generated based on lib/UnoCore/Source/Uno/Float4.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.UX.Markup.Types
{
    public struct Float4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Float4(float xyzw)
        {
            this.X = this.Y = this.Z = this.W = xyzw;
        }

        public Float4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Float4(Float2 xy, float z, float w)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
            this.W = w;
        }

        public Float4(float x, Float2 yz, float w)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
            this.W = w;
        }

        public Float4(float x, float y, Float2 zw)
        {
            this.X = x;
            this.Y = y;
            this.Z = zw.X;
            this.W = zw.Y;
        }

        public Float4(Float2 xy, Float2 zw)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = zw.X;
            this.W = zw.Y;
        }

        public Float4(Float3 xyz, float w)
        {
            this.X = xyz.X;
            this.Y = xyz.Y;
            this.Z = xyz.Z;
            this.W = w;
        }

        public Float4(float x, Float3 yzw)
        {
            this.X = x;
            this.Y = yzw.X;
            this.Z = yzw.Y;
            this.W = yzw.Z;
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
            return (((((this.X.ToString() + ", ") + this.Y.ToString()) + ", ") + this.Z.ToString()) + ", ") + this.W.ToString();
        }

        public static Float4 Identity
        {
            get { return new Float4(0.0f, 0.0f, 0.0f, 1.0f); }
        }

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return this.X;
                else if (index == 1)
                    return this.Y;
                else if (index == 2)
                    return this.Z;
                else if (index == 3)
                    return this.W;
                else
                    throw new global::System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    this.X = value;
                else if (index == 1)
                    this.Y = value;
                else if (index == 2)
                    this.Z = value;
                else if (index == 3)
                    this.W = value;
                else
                    throw new global::System.IndexOutOfRangeException();
            }
        }

        public static Float4 operator +(Float4 a, Float4 b)
        {
            return new Float4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Float4 operator -(Float4 a, Float4 b)
        {
            return new Float4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Float4 operator *(Float4 a, Float4 b)
        {
            return new Float4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

        public static Float4 operator /(Float4 a, Float4 b)
        {
            return new Float4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }

        public static Float4 operator +(Float4 a, float b)
        {
            return new Float4(a.X + b, a.Y + b, a.Z + b, a.W + b);
        }

        public static Float4 operator -(Float4 a, float b)
        {
            return new Float4(a.X - b, a.Y - b, a.Z - b, a.W - b);
        }

        public static Float4 operator *(Float4 a, float b)
        {
            return new Float4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Float4 operator /(Float4 a, float b)
        {
            return new Float4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }

        public static Float4 operator +(float a, Float4 b)
        {
            return new Float4(a + b.X, a + b.Y, a + b.Z, a + b.W);
        }

        public static Float4 operator -(float a, Float4 b)
        {
            return new Float4(a - b.X, a - b.Y, a - b.Z, a - b.W);
        }

        public static Float4 operator *(float a, Float4 b)
        {
            return new Float4(a * b.X, a * b.Y, a * b.Z, a * b.W);
        }

        public static Float4 operator /(float a, Float4 b)
        {
            return new Float4(a / b.X, a / b.Y, a / b.Z, a / b.W);
        }

        public static bool operator ==(Float4 a, Float4 b)
        {
            return (((a.X == b.X) && (a.Y == b.Y)) && (a.Z == b.Z)) && (a.W == b.W);
        }

        public static bool operator !=(Float4 a, Float4 b)
        {
            return (((a.X != b.X) || (a.Y != b.Y)) || (a.Z != b.Z)) || (a.W != b.W);
        }

        public static implicit operator Float4(Int4 a)
        {
            return new Float4((float)a.X, (float)a.Y, (float)a.Z, (float)a.W);
        }
    }
}
