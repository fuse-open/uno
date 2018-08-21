// This file was generated based on lib/UnoCore/Source/Uno/Rect.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct Recti
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Recti(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public Recti(Int2 pos, Int2 size)
        {
            this.Left = pos.X;
            this.Top = pos.Y;
            this.Right = this.Left + size.X;
            this.Bottom = this.Top + size.Y;
        }

        public static bool Equals(Recti rect1, Recti rect2)
        {
            return (((rect1.Left == rect2.Left) && (rect1.Top == rect2.Top)) && (rect1.Right == rect2.Right)) && (rect1.Bottom == rect2.Bottom);
        }

        public bool Contains(Recti r)
        {
            return (((this.Left <= r.Left) && (this.Right >= r.Right)) && (this.Top <= r.Top)) && (this.Bottom >= r.Bottom);
        }

        public bool Contains(Int2 p)
        {
            return (((p.X >= this.Left) && (p.X < this.Right)) && (p.Y >= this.Top)) && (p.Y < this.Bottom);
        }

        public bool Intersects(Recti r)
        {
            return (((r.Left < this.Right) && (r.Right > this.Left)) && (r.Top < this.Bottom)) && (r.Bottom > this.Top);
        }

        public override string ToString()
        {
            return (((((this.Left.ToString() + ", ") + this.Top.ToString()) + ", ") + this.Right.ToString()) + ", ") + this.Bottom.ToString();
        }

        public static Recti Union(Recti a, Recti b)
        {
            return new Recti(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
        }

        public static Recti Intersect(Recti a, Recti b)
        {
            return new Recti(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right), Math.Min(a.Bottom, b.Bottom));
        }

        public static Recti ContainingPoints(Int2[] points)
        {
            Int2[] array1;
            int index2;
            int length3;
            int minX = 2147483647;
            int maxX = -2147483648;
            int minY = 2147483647;
            int maxY = -2147483648;

            for (array1 = points, index2 = 0, length3 = array1.Length; index2 < length3; ++index2)
            {
                Int2 point = array1[index2];
                minX = Math.Min(minX, point.X);
                maxX = Math.Max(maxX, point.X);
                minY = Math.Min(minY, point.Y);
                maxY = Math.Max(maxY, point.Y);
            }

            return new Recti(minX, minY, maxX, maxY);
        }

        public static Recti Translate(Recti r, Int2 offset)
        {
            return new Recti(r.Left + offset.X, r.Top + offset.Y, r.Right + offset.X, r.Bottom + offset.Y);
        }

        public static Recti Inflate(Recti r, Int2 size)
        {
            return new Recti(r.Left - size.X, r.Top - size.Y, r.Right + size.X, r.Bottom + size.Y);
        }

        public static Recti Inflate(Recti r, int size)
        {
            return Recti.Inflate(r, new Int2(size, size));
        }

        public Int2 Minimum
        {
            get { return new Int2(this.Left, this.Top); }
            set
            {
                this.Left = value.X;
                this.Top = value.Y;
            }
        }

        public Int2 Maximum
        {
            get { return new Int2(this.Right, this.Bottom); }
            set
            {
                this.Right = value.X;
                this.Bottom = value.Y;
            }
        }

        public Int2 Center
        {
            get { return new Int2(this.Left + this.Right, this.Top + this.Bottom) / 2; }
        }

        public Int2 LeftTop
        {
            get { return new Int2(this.Left, this.Top); }
        }

        public Int2 RightTop
        {
            get { return new Int2(this.Right, this.Top); }
        }

        public Int2 LeftBottom
        {
            get { return new Int2(this.Left, this.Bottom); }
        }

        public Int2 RightBottom
        {
            get { return new Int2(this.Right, this.Bottom); }
        }

        public Int2 Position
        {
            get { return this.Minimum; }
            set
            {
                Int2 dp = value - this.Position;
                this.Left = value.X;
                this.Right = this.Right + dp.X;
                this.Top = value.Y;
                this.Bottom = this.Bottom + dp.Y;
            }
        }

        public Int2 Size
        {
            get { return new Int2(this.Right - this.Left, this.Bottom - this.Top); }
            set
            {
                this.Right = this.Left + value.X;
                this.Bottom = this.Top + value.Y;
            }
        }

        public int Area
        {
            get { return (this.Right - this.Left) * (this.Bottom - this.Top); }
        }

        public static explicit operator Recti(Rect r)
        {
            return new Recti((int)r.Left, (int)r.Top, (int)r.Right, (int)r.Bottom);
        }
    }
}
