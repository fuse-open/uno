using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Uno.UX.Markup
{
    public abstract class AtomicValue
    {
        public FileSourceInfo Source { get; }

        protected AtomicValue(FileSourceInfo source)
        {
            Source = source;
        }

        public abstract string ToLiteral();
    }

    public sealed class Bool : AtomicValue
    {
        public bool Value { get; }

        public Bool(bool b, FileSourceInfo srcInfo): base(srcInfo)
        {
            Value = b;
        }

        public override string ToLiteral()
        {
            return Value.ToString().ToLower();
        }
    }

    public sealed class String : AtomicValue
    {
        public string Value { get; }

        public String(string s, FileSourceInfo srcInfo)
            : base(srcInfo)
        {
            Value = s;
        }

        public override string ToLiteral()
        {
            return Value.ToLiteral();
        }
    }

    public abstract class Scalar : AtomicValue
    {
        protected Scalar(FileSourceInfo sourceInfo) : base(sourceInfo) {}

        public abstract object ObjectValue { get; }
    }

    public abstract class ScalarBase<T> : Scalar
    {
        public T Value { get; }

        public override object ObjectValue => Value;

        protected ScalarBase(T value, FileSourceInfo srcInfo)
            : base(srcInfo)
        {
            Value = value;
        }
    }

    public sealed class Scalar<T>: ScalarBase<T> where T : IFormattable
    {
        public Scalar(T value, FileSourceInfo srcInfo) : base(value, srcInfo) { }

        public override string ToLiteral()
        {
            if (typeof(T) == typeof(float))
            {
                return Value.ToString("G", CultureInfo.InvariantCulture) + "f";
            }
            else
            {
                return Value.ToString("G", CultureInfo.InvariantCulture);
            }
        }
    }

    public sealed class Size: AtomicValue
    {
        public float Value { get; }
        public string Unit { get; }
        public Size(float value, string unit, FileSourceInfo srcInfo): base(srcInfo)
        {
            Value = value;
            Unit = unit;
        }

        public override string ToLiteral()
        {
            if (Unit == "None") return "Uno.UX.Size.None";
            return "new Uno.UX.Size(" + Value.ToString("G", CultureInfo.InvariantCulture) + "f, Uno.UX.Unit." + Unit + ")";
        }
    }

    public sealed class Selector : AtomicValue
    {
        public string Value { get; }

        public Selector(string value, FileSourceInfo srcInfo) : base(srcInfo)
        {
            Value = value;
        }

        public override string ToLiteral()
        {
            return "new Uno.UX.Selector(" + Value.ToLiteral() + ")";
        }
    }

    public sealed class Size2: AtomicValue
    {
        public readonly Size X;
        public readonly Size Y;
        public Size2(Size x, Size y, FileSourceInfo srcInfo): base(srcInfo)
        {
            X = x;
            Y = y;
        }
        public override string ToLiteral()
        {
            return "new Uno.UX.Size2(" + X.ToLiteral() + ", " + Y.ToLiteral() + ")";
        }
    }

    public sealed class Vector<T> : AtomicValue where T : IFormattable
    {
        readonly Scalar<T>[] _comps;

        public IEnumerable<Scalar<T>> Components => _comps;

        public int ComponentCount => _comps.Length;

        public Scalar<T> this[int componentIndex] => _comps[componentIndex];

        public Vector(Scalar<T>[] comps, FileSourceInfo srcInfo)
            : base(srcInfo)
        {
            _comps = comps;
        }

        string CompType
        {
            get
            {
                if (typeof(T) == typeof(int)) return "int";
                if (typeof(T) == typeof(uint)) return "uint";
                if (typeof(T) == typeof(short)) return "short";
                if (typeof(T) == typeof(byte)) return "byte";
                if (typeof(T) == typeof(ushort)) return "short";
                if (typeof(T) == typeof(sbyte)) return "sbyte";
                if (typeof(T) == typeof(float)) return "float";
                if (typeof(T) == typeof(double)) return "double";

                throw new Exception("Unknown vector component type");
            }
        }

        public override string ToLiteral()
        {
            return CompType + _comps.Length + "(" + _comps.Select(x => x.ToLiteral()).Aggregate((x, y) => (x + ", " + y)) + ")";
        }
    }

}
