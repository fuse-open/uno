// This file was generated based on lib/UnoCore/Source/Uno/Float3.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.UX.Markup.Types
{
    public struct Float3
    {
        public float X;
        public float Y;
        public float Z;

        public Float3(float xyz)
        {
            this.X = this.Y = this.Z = xyz;
        }

        public Float3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Float3(Float2 xy, float z)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
        }

        public Float3(float x, Float2 yz)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
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
            return (((this.X.ToString() + ", ") + this.Y.ToString()) + ", ") + this.Z.ToString();
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
                else
                    throw new global::System.IndexOutOfRangeException();
            }
        }

        public static Float3 operator +(Float3 a, Float3 b)
        {
            return new Float3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Float3 operator -(Float3 a, Float3 b)
        {
            return new Float3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Float3 operator *(Float3 a, Float3 b)
        {
            return new Float3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Float3 operator /(Float3 a, Float3 b)
        {
            return new Float3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Float3 operator +(Float3 a, float b)
        {
            return new Float3(a.X + b, a.Y + b, a.Z + b);
        }

        public static Float3 operator -(Float3 a, float b)
        {
            return new Float3(a.X - b, a.Y - b, a.Z - b);
        }

        public static Float3 operator *(Float3 a, float b)
        {
            return new Float3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Float3 operator /(Float3 a, float b)
        {
            return new Float3(a.X / b, a.Y / b, a.Z / b);
        }

        public static Float3 operator +(float a, Float3 b)
        {
            return new Float3(a + b.X, a + b.Y, a + b.Z);
        }

        public static Float3 operator -(float a, Float3 b)
        {
            return new Float3(a - b.X, a - b.Y, a - b.Z);
        }

        public static Float3 operator *(float a, Float3 b)
        {
            return new Float3(a * b.X, a * b.Y, a * b.Z);
        }

        public static Float3 operator /(float a, Float3 b)
        {
            return new Float3(a / b.X, a / b.Y, a / b.Z);
        }

        public static bool operator ==(Float3 a, Float3 b)
        {
            return ((a.X == b.X) && (a.Y == b.Y)) && (a.Z == b.Z);
        }

        public static bool operator !=(Float3 a, Float3 b)
        {
            return ((a.X != b.X) || (a.Y != b.Y)) || (a.Z != b.Z);
        }

        public static Float3 operator -(Float3 a)
        {
            return a * -1.0f;
        }

        public static implicit operator Float3(Int3 a)
        {
            return new Float3((float)a.X, (float)a.Y, (float)a.Z);
        }
    }
}
