using Uno.Compiler.ExportTargetInterop;

namespace Uno.UX
{
	public enum Unit
	{
		Auto,
		Unspecified,
		Points,
		Pixels,
		Percent
	}

	/**
		Denotes a size in points, pixels or percent of available space.

		In UX Markup, values of this type can be written as follows:

		* `100pt` means 100 points.
		* `50%` means 50 percent of available space
		* `120px` means 120 physical pixels.
		* `100` means 100 unspecified units. Will default to points, unless a unit is implicit. For example, if set on a @Change animator
		  to animate a property already in percent, the value will be interprted as percent.

		If not specifying a value, many properties will default to `Size.Auto`, where the size will
		be computed automatically based on context. See the property documentation.
	*/
	public struct Size
	{
		public readonly float Value;
		public readonly Unit Unit;

		public Size(float value, Unit unit)
		{
			Value = value;
			Unit = unit;
		}

		public static implicit operator Size(float unspecifiedUnits)
		{
			return new Size(unspecifiedUnits, Unit.Unspecified);
		}

		public static implicit operator Size(int unspecifiedUnits)
		{
			return new Size(unspecifiedUnits, Unit.Unspecified);
		}

		public static explicit operator float(Size s)
		{
			return s.Value;
		}

		public static Unit Combine(Unit a, Unit b)
		{
			if (a == b) return a;
			if (a == Unit.Unspecified) return b;
			if (b == Unit.Unspecified) return a;
			return Unit.Unspecified;
		}

		public Unit DetermineUnit()
		{
			if (Unit == Unit.Unspecified) return Unit.Points;
			else return Unit;
		}

		public static Size operator + (Size a, Size b)
		{
			return new Size(a.Value + b.Value, Combine(a.Unit, b.Unit));
		}

		public static Size operator - (Size a, Size b)
		{
			return new Size(a.Value - b.Value, Combine(a.Unit, b.Unit));
		}

		public static Size operator * (Size a, float b)
		{
			return new Size(a.Value * b, a.Unit);
		}

		public static Size operator / (Size a, float b)
		{
			return new Size(a.Value / b, a.Unit);
		}

		public static Size operator * (Size a, Size b)
		{
			return new Size(a.Value * b.Value, Combine(a.Unit, b.Unit));
		}

		public static Size operator / (Size a, Size b)
		{
			return new Size(a.Value / b.Value, Combine(a.Unit, b.Unit));
		}

		public static bool operator == (Size a, Size b)
		{
			return a.Value == b.Value && a.Unit == b.Unit;
		}

		public static bool operator != (Size a, Size b)
		{
			return a.Value != b.Value || a.Unit != b.Unit;
		}

		public static Size Auto { get { return new Size(0, Unit.Auto); } }

		public bool IsAuto { get { return Unit == Unit.Auto; } }

		public static Size Points(float value)
		{
			return new Size(value, Unit.Points);
		}

		public static Size Pixels(float value)
		{
			return new Size(value, Unit.Pixels);
		}

		public static Size Percent(float value)
		{
			return new Size(value, Unit.Percent);
		}

		public override string ToString()
		{
			switch (Unit)
			{
				case Unit.Unspecified: return Value.ToString() + " (unspecified unit)";
				case Unit.Points: return Value.ToString() + "pt";
				case Unit.Pixels: return Value.ToString() + "px";
				case Unit.Percent: return Value.ToString() + "%";
				case Unit.Auto: return "(auto)";
				default: return Value.ToString() + " (" + Unit.ToString() + ")";
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Size)
			{
				var s = (Size)obj;
				if (s.Unit != Unit) return false;
				if (s.Value != Value) return false;
				return true;
			}
			else if (obj is float)
			{
				if (Unit != Unit.Points && Unit != Unit.Unspecified) return false;
				var f = (float)obj;
				if (f != Value) return false;
				return true;
			}
			else return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}

	/**
		Denotes a two-dimensional size in points, pixels or percent of available space.

		In UX Markup, values of this type can be written as two comma-separated @Size values.

		Examples:

		* `100pt, 50%` means 100 points on the horizontal axis, and 50% on the vertical axis
		* `100, 100` means 100 unspecified units in both directions. Will default to points, unless another unit is implict.

		@seealso Size
	*/
	public struct Size2
	{
		public readonly Size X;
		public readonly Size Y;

		public Size2(Size x, Size y)
		{
			X = x;
			Y = y;
		}

		public static implicit operator Size2(float2 unspecifiedUnitsVector)
		{
			return new Size2(unspecifiedUnitsVector.X, unspecifiedUnitsVector.Y);
		}

		public static implicit operator Size2(float unspecifiedUnits)
		{
			return new Size2(unspecifiedUnits, unspecifiedUnits);
		}

		public static explicit operator float2(Size2 v)
		{
			var x = v.X;
			var y = v.Y;
			return float2(x.Value, y.Value);
		}

		public static Size2 operator + (Size2 a, Size2 b)
		{
			return new Size2(a.X + b.X, a.Y + b.Y);
		}

		public static Size2 operator - (Size2 a, Size2 b)
		{
			return new Size2(a.X - b.X, a.Y - b.Y);
		}

		public static Size2 operator * (Size2 a, float b)
		{
			return new Size2(a.X * b, a.Y * b);
		}

		public static Size2 operator / (Size2 a, float b)
		{
			return new Size2(a.X / b, a.Y / b);
		}

		public static Size2 operator * (Size2 a, Size2 b)
		{
			return new Size2(a.X * b.X, a.Y * b.Y);
		}

		public static Size2 operator / (Size2 a, Size2 b)
		{
			return new Size2(a.X / b.X, a.Y / b.Y);
		}

		public static bool operator == (Size2 a, Size2 b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator != (Size2 a, Size2 b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		public static Size2 Auto
		{
			get { return new Size2(Size.Auto, Size.Auto); }
		}

		public static Size2 Points(float x, float y)
		{
			return new Size2(Size.Points(x), Size.Points(y));
		}

		public static Size2 Pixels(float x, float y)
		{
			return new Size2(Size.Pixels(x), Size.Pixels(y));
		}

		public static Size2 Percent(float x, float y)
		{
			return new Size2(Size.Percent(x), Size.Percent(y));
		}

		public override string ToString()
		{
			return X.ToString() + ", " + Y.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is Size2)
			{
				var s = (Size2)obj;
				if (s.X != X) return false;
				if (s.Y != Y) return false;
				return true;
			}
			else return false;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
	}
}
