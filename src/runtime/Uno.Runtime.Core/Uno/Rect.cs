// This file was generated based on Library/Core/UnoCore/Source/Uno/Rect.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public struct Rect
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public Rect(float left, float top, float right, float bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public Rect(Float2 pos, Float2 size)
        {
            this.Left = pos.X;
            this.Top = pos.Y;
            this.Right = float.IsInfinity(size.X) ? size.X : (this.Left + size.X);
            this.Bottom = float.IsInfinity(size.Y) ? size.Y : (this.Top + size.Y);
        }

        public static bool Equals(Rect rect1, Rect rect2)
        {
            return (((rect1.Left == rect2.Left) && (rect1.Top == rect2.Top)) && (rect1.Right == rect2.Right)) && (rect1.Bottom == rect2.Bottom);
        }

        public bool Contains(Rect r)
        {
            return (((this.Left <= r.Left) && (this.Right >= r.Right)) && (this.Top <= r.Top)) && (this.Bottom >= r.Bottom);
        }

        public bool Contains(Float2 p)
        {
            return (((this.Left <= p.X) && (this.Right >= p.X)) && (this.Top <= p.Y)) && (this.Bottom >= p.Y);
        }

        public bool Intersects(Rect r)
        {
            return !((((r.Left > this.Right) || (r.Right < this.Left)) || (r.Top > this.Bottom)) || (r.Bottom < this.Top));
        }

        public override string ToString()
        {
            return (((((this.Left.ToString() + ", ") + this.Top.ToString()) + ", ") + this.Right.ToString()) + ", ") + this.Bottom.ToString();
        }

        public static Rect Union(Rect a, Rect b)
        {
            return new Rect(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
        }

        public static Rect Intersect(Rect a, Rect b)
        {
            return new Rect(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right), Math.Min(a.Bottom, b.Bottom));
        }

        public static Rect Translate(Rect r, Float2 offset)
        {
            return new Rect(r.Left + offset.X, r.Top + offset.Y, r.Right + offset.X, r.Bottom + offset.Y);
        }

        public static Rect Scale(Rect r, Float2 scale)
        {
            return new Rect(r.Left * scale.X, r.Top * scale.Y, r.Right * scale.X, r.Bottom * scale.Y);
        }

        public static Rect Scale(Rect r, float scale)
        {
            return Rect.Scale(r, new Float2(scale, scale));
        }

        public static Rect Inflate(Rect r, Float2 size)
        {
            return new Rect(r.Left - size.X, r.Top - size.Y, r.Right + size.X, r.Bottom + size.Y);
        }

        public static Rect Inflate(Rect r, float size)
        {
            return Rect.Inflate(r, new Float2(size, size));
        }

        public static Rect ContainingPoints(Float2 point0, Float2 point1)
        {
            float minX = point0.X;
            float maxX = point0.X;
            float minY = point0.Y;
            float maxY = point0.Y;
            minX = Math.Min(minX, point1.X);
            maxX = Math.Max(maxX, point1.X);
            minY = Math.Min(minY, point1.Y);
            maxY = Math.Max(maxY, point1.Y);
            return new Rect(minX, minY, maxX, maxY);
        }

        public static Rect ContainingPoints(Float2 point0, Float2 point1, Float2 point2, Float2 point3)
        {
            float minX = point0.X;
            float maxX = point0.X;
            float minY = point0.Y;
            float maxY = point0.Y;
            minX = Math.Min(minX, point1.X);
            maxX = Math.Max(maxX, point1.X);
            minY = Math.Min(minY, point1.Y);
            maxY = Math.Max(maxY, point1.Y);
            minX = Math.Min(minX, point2.X);
            maxX = Math.Max(maxX, point2.X);
            minY = Math.Min(minY, point2.Y);
            maxY = Math.Max(maxY, point2.Y);
            minX = Math.Min(minX, point3.X);
            maxX = Math.Max(maxX, point3.X);
            minY = Math.Min(minY, point3.Y);
            maxY = Math.Max(maxY, point3.Y);
            return new Rect(minX, minY, maxX, maxY);
        }

        public bool IsInfinite
        {
            get { return ((float.IsInfinity(this.Left) || float.IsInfinity(this.Top)) || float.IsInfinity(this.Right)) || float.IsInfinity(this.Bottom); }
        }

        public Float2 Minimum
        {
            get { return new Float2(this.Left, this.Top); }
            set
            {
                this.Left = value.X;
                this.Top = value.Y;
            }
        }

        public Float2 Maximum
        {
            get { return new Float2(this.Right, this.Bottom); }
            set
            {
                this.Right = value.X;
                this.Bottom = value.Y;
            }
        }

        public Float2 Center
        {
            get { return new Float2(this.Left + this.Right, this.Top + this.Bottom) * 0.5f; }
        }

        public Float2 LeftTop
        {
            get { return new Float2(this.Left, this.Top); }
        }

        public Float2 RightTop
        {
            get { return new Float2(this.Right, this.Top); }
        }

        public Float2 LeftBottom
        {
            get { return new Float2(this.Left, this.Bottom); }
        }

        public Float2 RightBottom
        {
            get { return new Float2(this.Right, this.Bottom); }
        }

        public Float2 Position
        {
            get { return this.Minimum; }
            set
            {
                Float2 sz = this.Size;
                this.Left = value.X;
                this.Top = value.Y;
                this.Size = sz;
            }
        }

        public float Width
        {
            get { return float.IsInfinity(this.Right) ? this.Right : (this.Right - this.Left); }
            set { this.Right = float.IsInfinity(value) ? value : (this.Left + value); }
        }

        public float Height
        {
            get { return float.IsInfinity(this.Bottom) ? this.Bottom : (this.Bottom - this.Top); }
            set { this.Bottom = float.IsInfinity(value) ? value : (this.Top + value); }
        }

        public Float2 Size
        {
            get { return new Float2(this.Width, this.Height); }
            set
            {
                this.Width = value.X;
                this.Height = value.Y;
            }
        }

        public float Area
        {
            get { return this.Width * this.Height; }
        }

        public static implicit operator Rect(Recti r)
        {
            return new Rect((float)r.Left, (float)r.Top, (float)r.Right, (float)r.Bottom);
        }
    }
}
