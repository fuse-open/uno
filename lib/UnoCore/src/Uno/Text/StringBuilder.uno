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

        public StringBuilder()
        {
        }

        public StringBuilder(string value)
        {
            Append(value);
        }

        public override string ToString()
        @{
            uArray* data = @{$$._strings:Get()._data};
            size_t count = @{$$._strings:Get()._used};

            switch (count)
            {
                case 0:
                    return @{string.Empty};
                case 1:
                    return data->Unsafe<uString*>(0);
            }

            uString* result = uString::New(@{$$._totalLength});
            size_t offset = 0;

            for (size_t i = 0; i < count; i++)
            {
                uString* s = data->Unsafe<uString*>(i);
                memcpy(result->_ptr + offset, s->_ptr, s->_length * sizeof(@{char}));
                offset += s->_length;
            }

            U_ASSERT(offset == @{$$._totalLength});
            return result;
        @}

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
