// This file was generated based on lib/UnoCore/Source/Uno/Int2.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.UX.Markup.Types
{
    public struct Int2
    {
        public int X;
        public int Y;

        public Int2(int xy)
        {
            this.X = this.Y = xy;
        }

        public Int2(int x, int y)
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

        public int this[int index]
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
            get { return (float)this.X / (float)this.Y; }
        }

        public static Int2 operator +(Int2 a, Int2 b)
        {
            return new Int2(a.X + b.X, a.Y + b.Y);
        }

        public static Int2 operator -(Int2 a, Int2 b)
        {
            return new Int2(a.X - b.X, a.Y - b.Y);
        }

        public static Int2 operator *(Int2 a, Int2 b)
        {
            return new Int2(a.X * b.X, a.Y * b.Y);
        }

        public static Int2 operator /(Int2 a, Int2 b)
        {
            return new Int2(a.X / b.X, a.Y / b.Y);
        }

        public static Int2 operator +(Int2 a, int b)
        {
            return new Int2(a.X + b, a.Y + b);
        }

        public static Int2 operator -(Int2 a, int b)
        {
            return new Int2(a.X - b, a.Y - b);
        }

        public static Int2 operator *(Int2 a, int b)
        {
            return new Int2(a.X * b, a.Y * b);
        }

        public static Int2 operator /(Int2 a, int b)
        {
            return new Int2(a.X / b, a.Y / b);
        }

        public static bool operator ==(Int2 a, Int2 b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator !=(Int2 a, Int2 b)
        {
            return (a.X != b.X) || (a.Y != b.Y);
        }

        public static explicit operator Int2(Float2 v)
        {
            return new Int2((int)v.X, (int)v.Y);
        }
    }
}
