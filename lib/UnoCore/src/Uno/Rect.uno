using Uno.Compiler.ExportTargetInterop;
using Uno.Math;
using Uno.Vector;

namespace Uno
{
    public struct Rect
    {
        public float Left, Top, Right, Bottom;

        public Rect(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Rect(float2 pos, float2 size)
        {
            Left = pos.X;
            Top = pos.Y;
            Right = float.IsInfinity(size.X) ? size.X : Left + size.X;
            Bottom = float.IsInfinity(size.Y) ? size.Y : Top + size.Y;
        }

        public static bool Equals(Rect rect1, Rect rect2)
        {
            return rect1.Left == rect2.Left &&
                   rect1.Top == rect2.Top &&
                   rect1.Right == rect2.Right &&
                   rect1.Bottom == rect2.Bottom;
        }

        public bool IsInfinite
        {
            get { return float.IsInfinity(Left) || float.IsInfinity(Top) ||
                float.IsInfinity(Right) || float.IsInfinity(Bottom); }
        }

        public float2 Minimum
        {
            get { return float2(Left, Top); }
            set { Left = value.X; Top = value.Y; }
        }

        public float2 Maximum
        {
            get { return float2(Right, Bottom); }
            set { Right = value.X; Bottom = value.Y; }
        }

        public float2 Center
        {
            get { return float2(Left+Right, Top+Bottom) * 0.5f; }
        }

        public float2 LeftTop
        {
            get { return float2(Left,Top); }
        }

        public float2 RightTop
        {
            get { return float2(Right,Top); }
        }

        public float2 LeftBottom
        {
            get { return float2(Left,Bottom); }
        }

        public float2 RightBottom
        {
            get { return float2(Right,Bottom); }
        }

        public float2 Position
        {
            get { return Minimum; }
            set
            {
                var sz = Size;
                Left = value.X;
                Top = value.Y;
                Size = sz;
            }
        }

        public float Width
        {
            get { return float.IsInfinity(Right) ? Right : Right - Left; }
            set { Right = float.IsInfinity(value) ? value : Left + value; }
        }

        public float Height
        {
            get { return float.IsInfinity(Bottom) ? Bottom : Bottom - Top; }
            set { Bottom = float.IsInfinity(value) ? value : Top + value; }
        }

        public float2 Size
        {
            get { return float2(Width, Height); }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public float Area
        {
            get { return Width * Height; }
        }

        public bool Contains(Rect r)
        {
            return (Left <= r.Left) && (Right >= r.Right) && (Top <= r.Top) && (Bottom >= r.Bottom);
        }

        public bool Contains(float2 p)
        {
            return (Left <= p.X) && (Right >= p.X) && (Top <= p.Y) && (Bottom >= p.Y);
        }

        public bool Intersects(Rect r)
        {
            return !(r.Left > Right || r.Right < Left || r.Top > Bottom || r.Bottom < Top);
        }

        public override string ToString()
        {
            return Left.ToString() + ", " + Top.ToString() + ", " + Right.ToString() + ", " + Bottom.ToString();
        }

        public static implicit operator Rect(Recti r)
        {
            return new Rect(r.Left, r.Top, r.Right, r.Bottom);
        }

        public static Rect Union(Rect a, Rect b)
        {
            return new Rect(
                Min(a.Left, b.Left),
                Min(a.Top, b.Top),
                Max(a.Right, b.Right),
                Max(a.Bottom, b.Bottom));
        }

        public static Rect Intersect(Rect a, Rect b)
        {
            return new Rect(
                Max(a.Left, b.Left),
                Max(a.Top, b.Top),
                Min(a.Right, b.Right),
                Min(a.Bottom, b.Bottom));
        }

        [DotNetOverride]
        public static Rect Transform(Rect r, float4x4 matrix)
        {
            return Rect.ContainingPoints(
                TransformCoordinate(r.LeftTop, matrix),
                TransformCoordinate(r.RightTop, matrix),
                TransformCoordinate(r.RightBottom, matrix),
                TransformCoordinate(r.LeftBottom, matrix));
        }

        public static Rect Translate(Rect r, float2 offset)
        {
            return new Rect(
                r.Left + offset.X,
                r.Top + offset.Y,
                r.Right + offset.X,
                r.Bottom + offset.Y);
        }

        public static Rect Scale(Rect r, float2 scale)
        {
            return new Rect(
                r.Left * scale.X,
                r.Top * scale.Y,
                r.Right * scale.X,
                r.Bottom * scale.Y);
        }

        public static Rect Scale(Rect r, float scale)
        {
            return Scale(r, float2(scale, scale));
        }

        public static Rect Inflate(Rect r, float2 size)
        {
            return new Rect(
                r.Left - size.X,
                r.Top - size.Y,
                r.Right + size.X,
                r.Bottom + size.Y);
        }

        public static Rect Inflate(Rect r, float size)
        {
            return Inflate(r, float2(size, size));
        }

        // Too slow!
        /*

        public static Rect ContainingPoints(params float2[] points)
        {
            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;
            for (var point in points)
            {
                minX = Min(minX, point.X);
                maxX = Max(maxX, point.X);
                minY = Min(minY, point.Y);
                maxY = Max(maxY, point.Y);
            }
            return new Rect(minX,minY,maxX,maxY);
        }*/

        // JavaScript-optimized version for use (mainly) from Rect.Transform()
        public static Rect ContainingPoints(float2 point0, float2 point1)
        {
            var minX = point0.X;
            var maxX = point0.X;
            var minY = point0.Y;
            var maxY = point0.Y;

            minX = Min(minX, point1.X);
            maxX = Max(maxX, point1.X);
            minY = Min(minY, point1.Y);
            maxY = Max(maxY, point1.Y);

            return new Rect(minX,minY,maxX,maxY);
        }

        // JavaScript-optimized version for use (mainly) from Rect.Transform()
        public static Rect ContainingPoints(float2 point0, float2 point1, float2 point2, float2 point3)
        {
            var minX = point0.X;
            var maxX = point0.X;
            var minY = point0.Y;
            var maxY = point0.Y;

            minX = Min(minX, point1.X);
            maxX = Max(maxX, point1.X);
            minY = Min(minY, point1.Y);
            maxY = Max(maxY, point1.Y);

            minX = Min(minX, point2.X);
            maxX = Max(maxX, point2.X);
            minY = Min(minY, point2.Y);
            maxY = Max(maxY, point2.Y);

            minX = Min(minX, point3.X);
            maxX = Max(maxX, point3.X);
            minY = Min(minY, point3.Y);
            maxY = Max(maxY, point3.Y);

            return new Rect(minX,minY,maxX,maxY);
        }
    }

    public struct Recti
    {
        public int Left, Top, Right, Bottom;

        public Recti(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Recti(int2 pos, int2 size)
        {
            Left = pos.X;
            Top = pos.Y;
            Right = Left + size.X;
            Bottom = Top + size.Y;
        }

        public static bool Equals(Recti rect1, Recti rect2)
        {
            return rect1.Left == rect2.Left &&
                   rect1.Top == rect2.Top &&
                   rect1.Right == rect2.Right &&
                   rect1.Bottom == rect2.Bottom;
        }

        public int2 Minimum
        {
            get { return int2(Left, Top); }
            set { Left = value.X; Top = value.Y; }
        }

        public int2 Maximum
        {
            get { return int2(Right, Bottom); }
            set { Right = value.X; Bottom = value.Y; }
        }

        public int2 Center
        {
            get { return int2(Left+Right, Top+Bottom) / 2; }
        }

        public int2 LeftTop
        {
            get { return int2(Left,Top); }
        }

        public int2 RightTop
        {
            get { return int2(Right,Top); }
        }

        public int2 LeftBottom
        {
            get { return int2(Left,Bottom); }
        }

        public int2 RightBottom
        {
            get { return int2(Right,Bottom); }
        }

        public int2 Position
        {
            get { return Minimum; }
            set { int2 dp = value - Position; Left = value.X; Right += dp.X; Top = value.Y; Bottom += dp.Y; }
        }

        public int2 Size
        {
            get { return int2(Right - Left, Bottom - Top); }
            set { Right = Left + value.X; Bottom = Top + value.Y; }
        }

        public int Area
        {
            get { return (Right - Left) * (Bottom - Top); }
        }

        public bool Contains(Recti r)
        {
            return (Left <= r.Left) && (Right >= r.Right) && (Top <= r.Top) && (Bottom >= r.Bottom);
        }

        public bool Contains(int2 p)
        {
            return p.X >= Left && p.X < Right && p.Y >= Top && p.Y < Bottom;
        }

        public bool Intersects(Recti r)
        {
            return r.Left < Right && r.Right > Left && r.Top < Bottom && r.Bottom > Top;
        }

        public override string ToString()
        {
            return Left.ToString() + ", " + Top.ToString() + ", " + Right.ToString() + ", " + Bottom.ToString();
        }

        public static explicit operator Recti(Rect r)
        {
            return new Recti((int)r.Left, (int)r.Top, (int)r.Right, (int)r.Bottom);
        }

        public static Recti Union(Recti a, Recti b)
        {
            return new Recti(
                Min(a.Left, b.Left),
                Min(a.Top, b.Top),
                Max(a.Right, b.Right),
                Max(a.Bottom, b.Bottom));
        }

        public static Recti Intersect(Recti a, Recti b)
        {
            return new Recti(
                Max(a.Left, b.Left),
                Max(a.Top, b.Top),
                Min(a.Right, b.Right),
                Min(a.Bottom, b.Bottom));
        }

        public static Recti ContainingPoints(params int2[] points)
        {
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;
            foreach (var point in points)
            {
                minX = Min(minX, point.X);
                maxX = Max(maxX, point.X);
                minY = Min(minY, point.Y);
                maxY = Max(maxY, point.Y);
            }
            return new Recti(minX,minY,maxX,maxY);
        }

        public static Recti Translate(Recti r, int2 offset)
        {
            return new Recti(
                r.Left + offset.X,
                r.Top + offset.Y,
                r.Right + offset.X,
                r.Bottom + offset.Y);
        }

        public static Recti Inflate(Recti r, int2 size)
        {
            return new Recti(
                r.Left - size.X,
                r.Top - size.Y,
                r.Right + size.X,
                r.Bottom + size.Y);
        }

        public static Recti Inflate(Recti r, int size)
        {
            return Inflate(r, int2(size, size));
        }

    }
}
