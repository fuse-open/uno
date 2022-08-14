using Uno.Math;

namespace Uno.Time.Text
{
    internal interface IPatternPart<T>
    {
        int SkipNextPartsCountIfThisNotSpecified { get; }

        int Read(string line, int position, T value);

        string Write(T value);
    }

    internal abstract class PatternPart<TBucket, T> : IPatternPart<TBucket>
    {
        public Action<TBucket, T> Setter { get; protected set; }

        public Func<TBucket, T> Getter { get; protected set; }

        public int ExpectedSize { get; protected set; }

        public int SkipNextPartsCountIfThisNotSpecified { get; protected set; }

        public bool Obligatory { get; protected set; }

        public abstract int Read(string line, int position, TBucket value);

        public virtual string Write(TBucket value)
        {
            return Get(value).ToString();
        }

        protected void Set(TBucket bucket, T val)
        {
            if (Setter != null)
                Setter(bucket, val);
        }

        protected T Get(TBucket bucket)
        {
            if (Getter != null)
                return Getter(bucket);
            return default(T);
        }
    }

    internal sealed class NumberPart<T> : PatternPart<T, int>
    {
        public NumberPart(int size, bool obligatory, Action<T, int> setter, Func<T, int> getter)
        {
            Setter = setter;
            Getter = getter;
            ExpectedSize = size;
            Obligatory = obligatory;
        }

        public override int Read(string line, int position, T value)
        {
            var part = GetPart(line, position);
            if (string.IsNullOrEmpty(part))
            {
                if (Obligatory)
                    throw new FormatException("Not found number at " + position);
                return 0;
            }
            Set(value, int.Parse(part));
            return ExpectedSize;
        }

        private string GetPart(string line, int index)
        {
            if (index + ExpectedSize > line.Length)
            {
                if (Obligatory)
                    throw new FormatException("Not enough symbols at " + index);
                return string.Empty;
            }
            return line.Substring(index, ExpectedSize);
        }

        public override string Write(T value)
        {
            return string.Format("{0:D" + ExpectedSize + "}", Abs(Get(value)));
        }
    }

    internal sealed class RangeNumberPart<T> : PatternPart<T, int>
    {
        private int _maxSize;

        public RangeNumberPart(int maxSize, Action<T, int> setter, Func<T, int> getter)
            : this(0, maxSize, setter, getter)
        {
        }

        public RangeNumberPart(int size, int maxSize, Action<T, int> setter, Func<T, int> getter)
        {
            Setter = setter;
            Getter = getter;
            _maxSize = maxSize;
            ExpectedSize = size;
        }

        public override int Read(string line, int position, T value)
        {
            int numberCount = 0;
            for (var i = position; i < line.Length; i++)
            {
                if (!char.IsDigit(line[i]))
                    break;
                numberCount++;
                if (numberCount >= _maxSize)
                    break;
            }
            if (numberCount == 0)
                return 0;
            var number = int.Parse(line.Substring(position, numberCount > ExpectedSize ? ExpectedSize : numberCount));
            if (numberCount < ExpectedSize)
                number *= (int)Pow(10, ExpectedSize - numberCount);
            Set(value, number);
            return numberCount;
        }

        private string GetPart(string line, int index)
        {
            if (index + ExpectedSize > line.Length)
            {
                if (Obligatory)
                    throw new FormatException("Not enough symbols at " + index);
                return string.Empty;
            }
            return line.Substring(index, ExpectedSize);
        }

        public override string Write(T value)
        {
            return string.Empty;
        }
    }

    internal sealed class SignPart<T> : PatternPart<T, int>
    {
        public SignPart(Action<T, int> setter, Func<T, int> getter)
            : this(false, setter, getter)
        {
        }

        public SignPart(bool obligatory, Action<T, int> setter, Func<T, int> getter)
        {
            Setter = setter;
            Getter = getter;
            ExpectedSize = 1;
            Obligatory = obligatory;
        }

        public override int Read(string line, int position, T value)
        {
            if (position + 1 >= line.Length)
            {
                if (Obligatory)
                    throw new FormatException("Wrong number format");
                return 0;
            }
            var character = line[position];
            switch (character)
            {
                case '-':
                case '+':
                    Set(value, character == '-' ? -1 : 1);
                    return 1;
            }
            if (Obligatory)
                throw new FormatException("Wrong number format");
            Set(value, 1);
            return 0;
        }

        public override string Write(T value)
        {
            if (Get(value) == 1)
                return Obligatory ? "+": string.Empty;
            return "-";
        }
    }

    internal sealed class SeparatorPart<T> : PatternPart<T, char>
    {
        private char _separator;
        private bool _show;

        public SeparatorPart(bool obligatory, char separator, int skipNextPartsCountIfThisNotSpecified = 0)
            : this(obligatory, false, separator, skipNextPartsCountIfThisNotSpecified)
        {
        }

        public SeparatorPart(bool obligatory, bool show, char separator, int skipNextPartsCountIfThisNotSpecified = 0)
        {
            ExpectedSize = 1;
            Obligatory = obligatory;
            SkipNextPartsCountIfThisNotSpecified = skipNextPartsCountIfThisNotSpecified;
            _separator = separator;
            _show = show;
        }

        public override int Read(string line, int position, T value)
        {
            if (position + ExpectedSize >= line.Length)
            {
                if (Obligatory && position + ExpectedSize == line.Length)
                    throw new FormatException("Separator at the end");
                return 0;
            }
            if (line[position] == _separator)
            {
                Set(value, _separator);
                return 1;
            }
            if (Obligatory)
                throw new FormatException("Wrong number format");
            return 0;
        }

        public override string Write(T value)
        {
            return Obligatory || _show ? _separator.ToString() : string.Empty;
        }
    }

    internal sealed class OffsetPatternPart<T> : PatternPart<T, Offset>
    {
        public OffsetPatternPart(Action<T, Offset> setter, Func<T, Offset> getter)
        {
            Setter = setter;
            Getter = getter;
        }

        public override int Read(string line, int position, T value)
        {
            ParseResult<Offset> parse = OffsetPattern.GeneralIsoPattern.Parse(line.Substring(position));
            Set(value, parse.GetValueOrThrow());
            return line.Length - position;
        }

        public override string Write(T value)
        {
            return OffsetPattern.GeneralIsoPattern.Format(Get(value));
        }
    }
}
