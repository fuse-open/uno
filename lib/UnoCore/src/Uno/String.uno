using Uno.Compiler.ExportTargetInterop;
using Uno.Text;
using Uno.Collections;
using Uno.Internal;
using Uno.Math;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.String")]
    [extern(CPLUSPLUS) Set("TypeName", "uString*")]
    /** Represents text as a sequence of UTF-16 code units. */
    public sealed intrinsic class String : IEnumerable<char>
    {
        public static readonly string Empty = "";

        [extern(CPLUSPLUS) Set("FunctionName", "uString::CharArray")]
        [extern(CPLUSPLUS) Set("IsIntrinsic", "true")]
        public extern String(char[] str);

        [extern(CPLUSPLUS) Set("FunctionName", "uString::CharArrayRange")]
        [extern(CPLUSPLUS) Set("IsIntrinsic", "true")]
        public extern String(char[] str, int startIndex, int length);

        [extern(CPLUSPLUS) Set("IsIntrinsic", "true")]
        public extern int Length { get; }

        [extern(CPLUSPLUS) Set("IsIntrinsic", "true")]
        public extern char this[int index] { get; }

        public override int GetHashCode()
        {
            int hash = 5381;

            for (int i = 0; i < Length; i++)
                hash = ((hash << 5) + hash) ^ (int)this[i];

            return hash;
        }

        public override string ToString()
        {
            return this;
        }

        public string Replace(char oldChar, char newChar)
        @{
            uString* s = uString::New($$->_length);

            for (int i = 0; i < $$->_length; i++)
                s->_ptr[i] = $$->_ptr[i] == oldChar
                    ? newChar
                    : $$->_ptr[i];

            return s;
        @}

        public string Replace(string oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue == string.Empty)
                throw new ArgumentException("", nameof(oldValue));

            var index = IndexOf(oldValue);
            if (index == -1)
                return this;

            var sb = new StringBuilder();
            int pos = 0;

            while (index != -1)
            {
                sb.Append(SubCharArray(pos, index - pos));
                sb.Append(newValue);
                pos = index + oldValue.Length;
                index = IndexOf(oldValue, pos);
            }

            sb.Append(SubCharArray(pos, Length - pos));
            return sb.ToString();
        }

        private char[] SubCharArray(int start, int len)
        {
            var chars = new char[len];

            for (int i = 0; i < len; i++)
                chars[i] = this[start + i];

            return chars;
        }

        public string ToLower()
        @{
            uString* s = uString::New($$->_length);

            for (int i = 0; i < $$->_length; i++)
                s->_ptr[i] = @{Char.ToLower(char):Call($$->_ptr[i])};

            return s;
        @}

        public string ToUpper()
        @{
            uString* s = uString::New($$->_length);

            for (int i = 0; i < $$->_length; i++)
                s->_ptr[i] = @{Char.ToUpper(char):Call($$->_ptr[i])};

            return s;
        @}

        public override bool Equals(object other)
        @{
            if ($0 != nullptr && $$->__type == $0->__type)
            {
                uString* str = (uString*)$0;
                return $$->_length == str->_length &&
                    !memcmp($$->_ptr, str->_ptr, sizeof(char16_t) * $$->_length);
            }

            return false;
        @}

        public bool Equals(string other)
        {
            return Equals(this, other);
        }

        public static bool Equals(string left, string right)
        @{
            if ($0 == $1)
                return true;

            if (!$0 || !$1)
                return false;

            return $0->_length == $1->_length &&
                !memcmp($0->_ptr, $1->_ptr, sizeof(char16_t) * $0->_length);
        @}

        public static bool operator == (string left, string right)
        {
            return Equals(left, right);
        }

        public static bool operator != (string left, string right)
        {
            return !Equals(left, right);
        }

        public static string Concat(string a, string b)
        @{
            if (!$0 && !$1)
                return @{Empty};

            if (!$0)
                return $1;

            if (!$1)
                return $0;

            uString* s = uString::New($0->_length + $1->_length);
            memcpy(s->_ptr, $0->_ptr, $0->_length * sizeof(@{char}));
            memcpy(s->_ptr + $0->_length, $1->_ptr, $1->_length * sizeof(@{char}));
            return s;
        @}

        public static string Concat(object a, object b)
        {
            return Concat(a == null ? null : a.ToString(), b == null ? null : b.ToString());
        }

        public static string operator + (string a, string b)
        {
            return Concat(a, b);
        }

        public static string operator + (string a, object b)
        {
            return Concat(a, b);
        }

        public static string operator + (object a, string b)
        {
            return Concat(a, b);
        }

        public string Substring(int startIndex, int length)
        {
            if (startIndex > Length || startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex > Length - length || length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == 0)
                return Empty;

            @{
                uString* s = uString::New($1);
                memcpy(s->_ptr, $$->_ptr + $0, $1 * sizeof(@{char}));
                return s;
            @}
        }

        public string Substring(int start)
        {
            return Substring(start, Length - start);
        }

        public int IndexOf(char c)
        {
            return IndexOfUnchecked(c, 0, Length);
        }

        public int IndexOf(char c, int startIndex)
        {
            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return IndexOfUnchecked(c, startIndex, Length - startIndex);
        }

        public int IndexOf(char c, int startIndex, int count)
        {
            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0 || count > Length - startIndex)
                throw new ArgumentOutOfRangeException(nameof(count));

            return IndexOfUnchecked(c, startIndex, count);
        }

        int IndexOfUnchecked(char c, int startIndex, int count)
        {
            int length = startIndex + count;
            for (int i = startIndex; i < length; i++)
                if (this[i] == c)
                    return i;

            return -1;
        }

        public int IndexOfAny(char[] anyOf)
        {
            if (anyOf == null)
                throw new ArgumentNullException(nameof(anyOf));

            return IndexOfAnyUnchecked(anyOf, 0, Length);
        }

        public int IndexOfAny(char[] anyOf, int startIndex)
        {
            if (anyOf == null)
                throw new ArgumentNullException(nameof(anyOf));

            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return IndexOfAnyUnchecked(anyOf, startIndex, Length - startIndex);
        }

        public int IndexOfAny(char[] anyOf, int startIndex, int count)
        {
            if (anyOf == null)
                throw new ArgumentNullException(nameof(anyOf));

            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0 || count > Length - startIndex)
                throw new ArgumentOutOfRangeException(nameof(count));

            return IndexOfAnyUnchecked(anyOf, startIndex, count);
        }

        int IndexOfAnyUnchecked(char[] anyOf, int startIndex, int count)
        {
            int length = startIndex + count;
            for (int i = startIndex; i < length; i++)
                if (Array.IndexOf(anyOf, this[i]) >= 0)
                    return i;

            return -1;
        }

        public int LastIndexOf(char c)
        {
            return LastIndexOfUnchecked(c, Length - 1, Length);
        }

        public int LastIndexOf(char c, int startIndex)
        {
            if (Length == 0)
                return -1;

            if (startIndex < 0 || startIndex >= Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return LastIndexOfUnchecked(c, startIndex, startIndex + 1);
        }

        public int LastIndexOf(char c, int startIndex, int count)
        {
            if (Length == 0)
                return -1;

            if (startIndex < 0 || startIndex >= Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0 || count > startIndex + 1)
                throw new ArgumentOutOfRangeException(nameof(count));

            return LastIndexOfUnchecked(c, startIndex, count);
        }

        int LastIndexOfUnchecked(char c, int startIndex, int count)
        {
            for (int i = 0; i < count; ++i)
                if (this[startIndex - i] == c)
                    return startIndex - i;

            return -1;
        }

        public int LastIndexOfAny(char[] anyOf)
        {
            if (anyOf == null)
                throw new ArgumentNullException(nameof(anyOf));

            return LastIndexOfAnyUnchecked(anyOf, Length - 1, Length);
        }

        public int LastIndexOfAny(char[] anyOf, int startIndex)
        {
            if (anyOf == null)
                throw new ArgumentNullException(nameof(anyOf));

            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return LastIndexOfAnyUnchecked(anyOf, startIndex, startIndex + 1);
        }

        public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
        {
            if (anyOf == null)
                throw new ArgumentNullException(nameof(anyOf));

            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0 || count > startIndex + 1)
                throw new ArgumentOutOfRangeException(nameof(count));

            return LastIndexOfAnyUnchecked(anyOf, startIndex, count);
        }

        int LastIndexOfAnyUnchecked(char[] anyOf, int startIndex, int count)
        {
            for (int i = 0; i < count; ++i)
                if (Array.IndexOf(anyOf, this[startIndex - i]) >= 0)
                    return startIndex - i;

            return -1;
        }

        public bool StartsWith(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (Length < value.Length)
                return false;

            for (var i = 0; i < value.Length; i++)
                if (this[i] != value[i])
                    return false;

            return true;
        }

        public bool EndsWith(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (Length < value.Length)
                return false;

            var index = Length - value.Length;
            for (var i = 0; i < value.Length; i++)
                if (this[index++] != value[i])
                    return false;

            return true;
        }

        public char[] ToCharArray(int start, int length)
        {
            var result = new char[length];

            for (int i = 0; i < length; i++)
                result[i] = this[start + i];

            return result;
        }

        public char[] ToCharArray()
        {
            return ToCharArray(0, Length);
        }

        public string[] Split(params char[] splitChars)
        {
            if (splitChars == null || splitChars.Length == 0)
                splitChars = new[] { ' ', '\t', '\n', '\r' };

            int splitCount = 0;
            int charCount = 0;

            for (int i = 0; i < Length; i++)
                for (int k = 0; k < splitChars.Length; k++)
                    if (this[i] == splitChars[k])
                        splitCount++;

            var r = new string[splitCount + 1];
            var ch = new char[splitCount + 1][];

            splitCount = 0;
            int start = 0;

            for (int i = 0; i < Length; i++)
            {
                bool found = false;

                for (int k = 0; k < splitChars.Length; k++)
                {
                    if (this[i] == splitChars[k])
                    {
                        ch[splitCount] = new char[charCount];

                        for (int n = 0; n < charCount; n++)
                            ch[splitCount][n] = this[start+n];

                        start = i + 1;
                        splitCount++;
                        charCount = 0;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    charCount++;
            }

            ch[splitCount] = new char[charCount];

            for (int n = 0; n < charCount; n++)
                ch[splitCount][n] = this[start+n];

            for (int i = 0; i < ch.Length; i++)
                r[i] = new string(ch[i]);

            return r;
        }

        public static string Join(string separator, params object[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var strings = new string[value.Length];

            for (int i = 0; i < value.Length; i++)
                strings[i] = value[i] != null
                                ? value[i].ToString()
                                : null;

            return Join(separator, strings);
        }

        public static string Join(string separator, params string[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            switch (value.Length)
            {
                case 0:
                    return Empty;
                case 1:
                    return value[0] ?? Empty;
            }

            if (separator == null)
                separator = Empty;

            @{
                size_t length = separator->_length * (value->_length - 1);

                for (size_t i = 0; i < value->_length; i++)
                    if (value->Unsafe<uString*>(i))
                        length += value->Unsafe<uString*>(i)->_length;

                uString* result = uString::New(length);
                size_t offset = 0;

                for (size_t i = 0; i < value->_length; i++)
                {
                    if (i > 0)
                    {
                        memcpy(result->_ptr + offset, separator->_ptr, separator->_length * sizeof(@{char}));
                        offset += separator->_length;
                    }

                    if (value->Unsafe<uString*>(i))
                    {
                        memcpy(result->_ptr + offset, value->Unsafe<uString*>(i)->_ptr, value->Unsafe<uString*>(i)->_length * sizeof(@{char}));
                        offset += value->Unsafe<uString*>(i)->_length;
                    }
                }

                U_ASSERT(offset == length);
                return result;
            @}
        }

        public static bool IsNullOrEmpty (string s)
        {
            return s == null || s == Empty;
        }

        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null)
                return true;

            for (int i = 0; i < value.Length; i++)
                if (!char.IsWhiteSpace(value[i]))
                    return false;

            return true;
        }

        public string PadLeft(int totalLength)
        {
            return PadLeft(totalLength, ' ');
        }

        public string PadLeft(int totalLength, char paddingSymbol)
        @{
            int padLength = $0 - $$->Length();
            if (padLength <= 0)
                return $$;

            uString* result = uString::New($0);

            for (int i = 0; i < padLength; i++)
                result->_ptr[i] = $1;

            memcpy(result->_ptr + padLength, $$->_ptr, $$->_length * sizeof(@{char}));
            return result;
        @}

        public string PadRight(int totalLength)
        {
            return PadRight(totalLength, ' ');
        }

        public string PadRight(int totalLength, char paddingSymbol)
        @{
            if ($0 <= $$->Length())
                return $$;

            uString* result = uString::New($0);
            memcpy(result->_ptr, $$->_ptr, $$->_length * sizeof(@{char}));

            for (int i = $$->Length(); i < $0; i++)
                result->_ptr[i] = $1;

            return result;
        @}

        public string Trim()
        {
            if (this == Empty)
                return Empty;

            var first = IndexOfFirstNotWhiteSpace();
            if (first == -1)
                return Empty;

            var last = IndexOfLastNotWhiteSpace();
            var length = last - first + 1;
            return Substring(first, length);
        }

        public string Trim(params char[] trimChars)
        {
            if (this == Empty)
                return Empty;

            var first = IndexOfFirstNotInSet(trimChars);
            if (first == -1)
                return Empty;

            var last = IndexOfLastNotInSet(trimChars);
            var length = last - first + 1;
            return Substring(first, length);
        }

        public string TrimStart(params char[] trimChars)
        {
            if (this == Empty)
                return Empty;

            if (trimChars.Length == 0)
                return TrimStartWhiteSpace();

            var first = IndexOfFirstNotInSet(trimChars);
            if (first == -1)
                return Empty;

            return Substring(first);
        }

        private string TrimStartWhiteSpace()
        {
            if (this == Empty)
                return Empty;

            var first = IndexOfFirstNotWhiteSpace();
            if (first == -1)
                return Empty;

            return Substring(first);
        }

        public string TrimEnd(params char[] trimChars)
        {
            if (this == Empty)
                return Empty;

            if (trimChars.Length == 0)
                return TrimEndWhiteSpace();

            var last = IndexOfLastNotInSet(trimChars);
            return Substring(0, last + 1);
        }

        private string TrimEndWhiteSpace()
        {
            if (this == Empty)
                return Empty;

            var last = IndexOfLastNotWhiteSpace();
            return Substring(0, last + 1);
        }

        int IndexOfFirstNotInSet(char[] charSet)
        {
            for (int i = 0; i < Length; i++)
                if (!InSet(this[i], charSet))
                    return i;

            return -1;
        }

        int IndexOfLastNotInSet(char[] charSet)
        {
            for (int i = Length - 1; i >= 0; i--)
                if (!InSet(this[i], charSet))
                    return i;

            return -1;
        }

        bool InSet(char c, char[] charSet)
        {
            for (int i = 0; i < charSet.Length; i++)
                if (charSet[i] == c)
                    return true;

            return false;
        }

        private int IndexOfFirstNotWhiteSpace()
        {
            for (int i = 0; i < Length; i++)
                if (!char.IsWhiteSpace(this[i]))
                    return i;

            return -1;
        }

        private int IndexOfLastNotWhiteSpace()
        {
            for (int i = Length - 1; i >= 0; i--)
                if (!char.IsWhiteSpace(this[i]))
                    return i;

            return -1;
        }

        public string Insert(int pos, string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (pos < 0 || pos > Length)
                throw new ArgumentOutOfRangeException(nameof(pos));

            if (str.Length == 0)
                return this;

            if (Length == 0)
                return str;

            @{
                uString* s = uString::New($$->_length + $1->_length);
                memcpy(s->_ptr, $$->_ptr, pos * sizeof(@{char}));
                memcpy(s->_ptr + pos, $1->_ptr, $1->_length * sizeof(@{char}));
                memcpy(s->_ptr + pos + $1->_length, $$->_ptr + pos, ($$->_length - pos) * sizeof(@{char}));
                return s;
            @}
        }

        public int IndexOf(string str, int startIndex=0)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (str == Empty)
                return 0;

            if (startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            for (int hay = startIndex; hay < Length; hay++)
            {
                if (str.Length > (Length - hay))
                    return -1;
                if (MatchesAt(str, hay))
                    return hay;
            }

            return -1;
        }

        private bool MatchesAt(string str, int pos)
        {
            for (int i = 0; i < str.Length; i++)
                if (pos + i == Length || this[pos+i] != str[i])
                    return false;

            return true;
        }

        public static int Compare(string a, string b)
        {
            for (int i = 0; i < Min(a.Length, b.Length); i++)
            {
                if (a[i] < b[i])
                    return -1;

                if (b[i] < a[i])
                    return 1;
            }

            if (a.Length < b.Length)
                return -1;

            if (b.Length < a.Length)
                return 1;

            return 0;
        }

        [DotNetOverride]
        public static string Format(string str, params object[] objs)
        {
            var builder = new StringBuilder();
            var tokens = FormatStringTokenizer.TokenizeFormatString(str);

            foreach (var token in tokens)
                builder.Append(token.ToString(objs));

            return builder.ToString();
        }

        public bool Contains(string str)
        {
            return IndexOf(str, 0) >= 0;
        }

        public IEnumerator<char> GetEnumerator()
        {
            return new Enumerator(this);
        }

        class Enumerator : IEnumerator<char>
        {
            readonly string _source;
            char _current;
            int _iterator;

            public Enumerator(string source)
            {
                _source = source;
                _iterator = -1;
            }

            public char Current
            {
                get { return _current; }
            }

            public void Dispose()
            {
            }

            public void Reset()
            {
                _iterator = -1;
                _current = '\0';
            }

            public bool MoveNext()
            {
                _iterator++;

                if (_iterator < _source.Length)
                {
                    _current = _source[_iterator];
                    return true;
                }

                return false;
            }
        }
    }
}
