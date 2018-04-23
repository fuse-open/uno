using Uno.Text;

namespace Uno.Time.Text
{
    sealed class FixedFormatPattern<T>
    {
        private IPatternPart<T>[] _parts;

        public FixedFormatPattern(IPatternPart<T>[] parts)
        {
            _parts = parts;
        }

        public void Parse(string line, T bucket)
        {
            int position = 0;

            for(int i = 0; i < _parts.Length; i++)
            {
                var part = _parts[i];
                var readCount = part.Read(line, position, bucket);
                position += readCount;

                if (readCount == 0)
                    i += part.SkipNextPartsCountIfThisNotSpecified;
            }
            if (position != line.Length)
            {
                throw new FormatException("Line is too long");
            }
        }

        public string Format(T bucket)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var part in _parts)
            {
                sb.Append(part.Write(bucket));
            }
            return sb.ToString();
        }
    }
}
