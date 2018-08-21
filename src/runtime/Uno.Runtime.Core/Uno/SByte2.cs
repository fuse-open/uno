// This file was generated based on lib/UnoCore/Source/Uno/SByte2.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct SByte2
    {
        public sbyte X;
        public sbyte Y;

        public SByte2(sbyte x, sbyte y)
        {
            this.X = x;
            this.Y = y;
        }

        public SByte2(sbyte xy)
        {
            this.X = this.Y = xy;
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

        public sbyte this[int index]
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

        public static Int2 operator +(SByte2 a, SByte2 b)
        {
            return new Int2(a.X + b.X, a.Y + b.Y);
        }

        public static Int2 operator -(SByte2 a, SByte2 b)
        {
            return new Int2(a.X - b.X, a.Y - b.Y);
        }

        public static Int2 operator *(SByte2 a, SByte2 b)
        {
            return new Int2(a.X * b.X, a.Y * b.Y);
        }

        public static Int2 operator /(SByte2 a, SByte2 b)
        {
            return new Int2(a.X / b.X, a.Y / b.Y);
        }

        public static Int2 operator +(SByte2 a, sbyte b)
        {
            return new Int2(a.X + b, a.Y + b);
        }

        public static Int2 operator -(SByte2 a, sbyte b)
        {
            return new Int2(a.X - b, a.Y - b);
        }

        public static Int2 operator *(SByte2 a, sbyte b)
        {
            return new Int2(a.X * b, a.Y * b);
        }

        public static Int2 operator /(SByte2 a, sbyte b)
        {
            return new Int2(a.X / b, a.Y / b);
        }

        public static bool operator ==(SByte2 a, SByte2 b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator !=(SByte2 a, SByte2 b)
        {
            return (a.X != b.X) || (a.Y != b.Y);
        }

        public static explicit operator SByte2(Byte2 v)
        {
            return new SByte2((sbyte)v.X, (sbyte)v.Y);
        }

        public static explicit operator SByte2(Short2 v)
        {
            return new SByte2((sbyte)v.X, (sbyte)v.Y);
        }

        public static explicit operator SByte2(UShort2 v)
        {
            return new SByte2((sbyte)v.X, (sbyte)v.Y);
        }

        public static explicit operator SByte2(Int2 v)
        {
            return new SByte2((sbyte)v.X, (sbyte)v.Y);
        }

        public static explicit operator SByte2(Float2 v)
        {
            return new SByte2((sbyte)v.X, (sbyte)v.Y);
        }
    }
}
