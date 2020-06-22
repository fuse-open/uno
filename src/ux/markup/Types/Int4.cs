// This file was generated based on lib/UnoCore/Source/Uno/Int4.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.UX.Markup.Types
{
    public struct Int4
    {
        public int X;
        public int Y;
        public int Z;
        public int W;

        public Int4(int x, int y, int z, int w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Int4(Int2 xy, int z, int w)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
            this.W = w;
        }

        public Int4(int x, Int2 yz, int w)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
            this.W = w;
        }

        public Int4(int x, int y, Int2 zw)
        {
            this.X = x;
            this.Y = y;
            this.Z = zw.X;
            this.W = zw.Y;
        }

        public Int4(Int2 xy, Int2 zw)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = zw.X;
            this.W = zw.Y;
        }

        public Int4(Int3 xyz, int w)
        {
            this.X = xyz.X;
            this.Y = xyz.Y;
            this.Z = xyz.Z;
            this.W = w;
        }

        public Int4(int x, Int3 yzw)
        {
            this.X = x;
            this.Y = yzw.X;
            this.Z = yzw.Y;
            this.W = yzw.Z;
        }

        public Int4(int xyzw)
        {
            this.X = xyzw;
            this.Y = xyzw;
            this.Z = xyzw;
            this.W = xyzw;
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

        public int this[int index]
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

        public static Int4 operator +(Int4 a, Int4 b)
        {
            return new Int4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Int4 operator -(Int4 a, Int4 b)
        {
            return new Int4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Int4 operator *(Int4 a, Int4 b)
        {
            return new Int4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

        public static Int4 operator /(Int4 a, Int4 b)
        {
            return new Int4(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }

        public static Int4 operator +(Int4 a, int b)
        {
            return new Int4(a.X + b, a.Y + b, a.Z + b, a.W + b);
        }

        public static Int4 operator -(Int4 a, int b)
        {
            return new Int4(a.X - b, a.Y - b, a.Z - b, a.W - b);
        }

        public static Int4 operator *(Int4 a, int b)
        {
            return new Int4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Int4 operator /(Int4 a, int b)
        {
            return new Int4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }

        public static bool operator ==(Int4 a, Int4 b)
        {
            return (((a.X == b.X) && (a.Y == b.Y)) && (a.Z == b.Z)) && (a.W == b.W);
        }

        public static bool operator !=(Int4 a, Int4 b)
        {
            return (((a.X != b.X) || (a.Y != b.Y)) || (a.Z != b.Z)) || (a.W != b.W);
        }

        public static explicit operator Int4(Float4 v)
        {
            return new Int4((int)v.X, (int)v.Y, (int)v.Z, (int)v.W);
        }
    }
}
