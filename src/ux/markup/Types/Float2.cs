// This file was generated based on lib/UnoCore/Source/Uno/Float2.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.UX.Markup.Types
{
    public struct Float2
    {
        public float X;
        public float Y;

        public Float2(float xy)
        {
            this.X = this.Y = xy;
        }

        public Float2(float x, float y)
        {
            this.X = x;
            this.Y = y;
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
            return (this.X.ToString() + ", ") + this.Y.ToString();
        }

        public float this[int index]
        {
            get
            {
                if (index == 0)
                    return this.X;
                else if (index == 1)
                    return this.Y;
                else
                    throw new global::System.IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    this.X = value;
                else if (index == 1)
                    this.Y = value;
                else
                    throw new global::System.IndexOutOfRangeException();
            }
        }

        public float Ratio
        {
            get { return this.X / this.Y; }
        }

        public static Float2 operator -(Float2 a)
        {
            return new Float2(-a.X, -a.Y);
        }

        public static Float2 operator +(Float2 a, Float2 b)
        {
            return new Float2(a.X + b.X, a.Y + b.Y);
        }

        public static Float2 operator -(Float2 a, Float2 b)
        {
            return new Float2(a.X - b.X, a.Y - b.Y);
        }

        public static Float2 operator *(Float2 a, Float2 b)
        {
            return new Float2(a.X * b.X, a.Y * b.Y);
        }

        public static Float2 operator /(Float2 a, Float2 b)
        {
            return new Float2(a.X / b.X, a.Y / b.Y);
        }

        public static Float2 operator +(Float2 a, float b)
        {
            return new Float2(a.X + b, a.Y + b);
        }

        public static Float2 operator -(Float2 a, float b)
        {
            return new Float2(a.X - b, a.Y - b);
        }

        public static Float2 operator *(Float2 a, float b)
        {
            return new Float2(a.X * b, a.Y * b);
        }

        public static Float2 operator /(Float2 a, float b)
        {
            return new Float2(a.X / b, a.Y / b);
        }

        public static Float2 operator +(float a, Float2 b)
        {
            return new Float2(a + b.X, a + b.Y);
        }

        public static Float2 operator -(float a, Float2 b)
        {
            return new Float2(a - b.X, a - b.Y);
        }

        public static Float2 operator *(float a, Float2 b)
        {
            return new Float2(a * b.X, a * b.Y);
        }

        public static Float2 operator /(float a, Float2 b)
        {
            return new Float2(a / b.X, a / b.Y);
        }

        public static bool operator ==(Float2 a, Float2 b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator !=(Float2 a, Float2 b)
        {
            return (a.X != b.X) || (a.Y != b.Y);
        }

        public static implicit operator Float2(Int2 a)
        {
            return new Float2((float)a.X, (float)a.Y);
        }
    }
}
