// This file was generated based on lib/UnoCore/Source/Uno/Int3.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno.UX.Markup.Types
{
    public struct Int3
    {
        public int X;
        public int Y;
        public int Z;

        public Int3(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Int3(Int2 xy, int z)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
        }

        public Int3(int x, Int2 yz)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
        }

        public Int3(int xyz)
        {
            this.X = xyz;
            this.Y = xyz;
            this.Z = xyz;
        }

        public override string ToString()
        {
            return (((this.X.ToString() + ", ") + this.Y.ToString()) + ", ") + this.Z.ToString();
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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

        public static Int3 operator +(Int3 a, Int3 b)
        {
            return new Int3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Int3 operator -(Int3 a, Int3 b)
        {
            return new Int3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Int3 operator *(Int3 a, Int3 b)
        {
            return new Int3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Int3 operator /(Int3 a, Int3 b)
        {
            return new Int3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Int3 operator +(Int3 a, int b)
        {
            return new Int3(a.X + b, a.Y + b, a.Z + b);
        }

        public static Int3 operator -(Int3 a, int b)
        {
            return new Int3(a.X - b, a.Y - b, a.Z - b);
        }

        public static Int3 operator *(Int3 a, int b)
        {
            return new Int3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Int3 operator /(Int3 a, int b)
        {
            return new Int3(a.X / b, a.Y / b, a.Z / b);
        }

        public static bool operator ==(Int3 a, Int3 b)
        {
            return ((a.X == b.X) && (a.Y == b.Y)) && (a.Z == b.Z);
        }

        public static bool operator !=(Int3 a, Int3 b)
        {
            return ((a.X != b.X) || (a.Y != b.Y)) || (a.Z != b.Z);
        }

        public static explicit operator Int3(Float3 v)
        {
            return new Int3((int)v.X, (int)v.Y, (int)v.Z);
        }
    }
}
