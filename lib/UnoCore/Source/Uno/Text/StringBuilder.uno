using Uno.Compiler.ExportTargetInterop;
using Uno.Collections;

namespace Uno.Text
{
    [DotNetType("System.Text.StringBuilder")]
    public class StringBuilder
    {
        int _totalLength = 0;
        List<string> _strings = new List<string>();

        public int MaxCapacity { get { return int.MaxValue; }}

        public override string ToString()
        {
            var c = new char[_totalLength];
            int x = 0;
            for (int i = 0; i < _strings.Count; i++)
            {
                var s = _strings[i];
                for (var n = 0; n < s.Length; n++)
                    c[x++] = s[n];
            }

            return new string(c);
        }

        public int Length { get { return _totalLength; } }

        public StringBuilder Append(char[] chars)
        {
            if (MaxCapacity - Length < chars.Length)
                throw new ArgumentOutOfRangeException(nameof(chars));

            if (chars.Length > 0)
            {
                _strings.Add(new string(chars)); // convert to string to make immutable
                _totalLength += chars.Length;
            }
            return this;
        }

        public StringBuilder Append(string str)
        {
            if (str.Length > 0)
            {
                _strings.Add(str);
                _totalLength += str.Length;
            }
            return this;
        }

        public StringBuilder Append(char c)
        {
            return Append(new char[] { c });
        }

        public StringBuilder AppendLine(string str)
        {
            return Append(str).Append(Environment.NewLine);
        }
    }
}
