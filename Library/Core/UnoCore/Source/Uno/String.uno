using Uno.Compiler.ExportTargetInterop;
using Uno.Text;
using Uno.Collections;
using Uno.Runtime.Implementation.Internal;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.String")]
    [extern(CPLUSPLUS) Set("TypeName", "uString*")]
    [extern(JAVASCRIPT) Set("TargetTypeName", "String")]
    /** Represents text as a sequence of UTF-16 code units. */
    public sealed intrinsic class String
    {
        public static readonly string Empty = "";

        [extern(CPLUSPLUS) Set("FunctionName", "uString::CharArray")]
        [extern(CPLUSPLUS || JAVASCRIPT) Set("IsIntrinsic", "true")]
        public extern String(char[] str);

        [extern(CPLUSPLUS) Set("FunctionName", "uString::CharArrayRange")]
        [extern(CPLUSPLUS) Set("IsIntrinsic", "true")]
        public extern String(char[] str, int startIndex, int length);

        [extern(CPLUSPLUS || JAVASCRIPT) Set("IsIntrinsic", "true")]
        public extern int Length
        {
            get;
        }

        [extern(CPLUSPLUS || JAVASCRIPT) Set("IsIntrinsic", "true")]
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
        {
            var s = new char[Length];
            for (int i = 0; i < Length; i++)
            {
                s[i] = this[i];
                if (s[i] == oldChar) s[i] = newChar;
            }
            return new string(s);
        }

        public string Replace(string oldValue, string newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException(nameof(oldValue));

            if (oldValue == string.Empty)
                throw new ArgumentException("", nameof(oldValue));

            var index = IndexOf(oldValue);
            if (index == -1)
            {
                return this;
            }

            var sb = new StringBuilder();
            int pos = 0;

            while (index != -1)
            {
                sb.Append(SubCharArray(pos, index-pos));
                sb.Append(newValue);
                pos = index + oldValue.Length;
                index = IndexOf(oldValue, pos);
            }
            sb.Append(SubCharArray(pos, Length-pos));

            return sb.ToString();
        }

        private char[] SubCharArray(int start, int len)
        {
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i]= this[start+i];
            }
            return chars;
        }

        public string ToLower()
        {
            if defined(JAVASCRIPT)
            @{
                return $$.toLowerCase();
            @}
            else
            {
                var chars = new char[Length];
                for (int i = 0; i < Length; i++)
                {
                    chars[i] = Char.ToLower(this[i]);
                }
                return new string(chars);
            }
        }

        public string ToUpper()
        {
            if defined(JAVASCRIPT)
            @{
                return $$.toUpperCase();
            @}
            else
            {
                var chars = new char[Length];
                for (int i = 0; i < Length; i++)
                {
                    chars[i] = Char.ToUpper(this[i]);
                }
                return new string(chars);
            }
        }

        public override bool Equals(object other)
        {
            if defined(CPLUSPLUS)
            @{
                if ($0 != NULL && $$->__type == $0->__type)
                {
                    uString* str = (uString*)$0;
                    return $$->_length == str->_length && !memcmp($$->_ptr, str->_ptr, sizeof(char16_t) * $$->_length);
                }

                return false;
            @}
            else
                return Equals(this, other as string);
        }

        public bool Equals(string other)
        {
            return Equals(this, other);
        }

        public static bool Equals(string left, string right)
        {
            if (object.ReferenceEquals(left, right))
                return true;

            if (object.ReferenceEquals(left, null) ||
                object.ReferenceEquals(right, null))
                return false;

            if (left.Length != right.Length)
                return false;

            for (int i = 0; i < left.Length; i++)
                if (left[i] != right[i])
                    return false;

            return true;
        }

        public static bool operator == (string left, string right)
        {
            return Equals(left, right);
        }

        public static bool operator != (string left, string right)
        {
            return !Equals(left, right);
        }

        public static string Concat(string a, string b)
        {
            if defined(JAVASCRIPT)
            @{
                var an = @{Uno.Object.ReferenceEquals(object,object):Call($0,null)};
                var bn = @{Uno.Object.ReferenceEquals(object,object):Call($1,null)};
                if (an == true && bn == true)
                    return @{string.Empty};
                if (an == true)
                    return $1;
                if (bn == true)
                    return $0;
                var c = new Array($0.length + $1.length);
                for (var i = 0; i < $0.length; i++)
                    c[i] = $0.charCodeAt(i);
                for (var i = 0; i < $1.length; i++)
                    c[i + $0.length] = $1.charCodeAt(i);
                return @{string(char[]):New(c)};
            @}
            else
            {
                if (object.ReferenceEquals(a, null) &&
                    object.ReferenceEquals(b, null))
                    return Empty;

                if (object.ReferenceEquals(a, null))
                    return b;

                if (object.ReferenceEquals(b, null))
                    return a;

                var s = new char[a.Length + b.Length];

                for (int i = 0; i < a.Length; i++)
                    s[i] = a[i];

                for (int i = 0; i < b.Length; i++)
                    s[a.Length + i] = b[i];

                return new string(s);
            }
        }

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

            if defined(JAVASCRIPT)
            @{
                return $$.substr($0, $1);
            @}
            else
            {
                if (startIndex == Length && length == 0)
                    return Empty;

                var s = new char[length];

                for (int i = 0; i < length; i++)
                    s[i] = this[startIndex + i];

                return new string(s);
            }
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
            return LastIndexOfUnchecked(c, this.Length - 1, this.Length);
        }

        public int LastIndexOf(char c, int startIndex)
        {
            if (this.Length == 0)
                return -1;

            if (startIndex < 0 || startIndex >= Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return LastIndexOfUnchecked(c, startIndex, startIndex + 1);
        }

        public int LastIndexOf(char c, int startIndex, int count)
        {
            if (this.Length == 0)
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
            {
                if (this[i] != value[i])
                    return false;
            }
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
            {
                if (this[index++] != value[i])
                    return false;
            }
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

        public static string Join(string separator, params string[] value)
        {
            var result = "";

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0)
                    result += separator;

                result += value[i];
            }

            return result;
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
            {
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            }
            return true;
        }

        public string PadLeft(int totalLength)
        {
            return PadLeft(totalLength, ' ');
        }

        public string PadLeft(int totalLength, char paddingSymbol)
        {
            int padLength = totalLength - Length;
            if (padLength <= 0)
                return this;
            var result = new char[totalLength];
            int index;
            for (index = 0; index < padLength; index++)
            {
                result[index] = paddingSymbol;
            }
            for (var i = 0; i < Length; i++)
            {
                result[index++] = this[i];
            }
            return new string(result);
        }

        public string PadRight(int totalLength)
        {
            return PadRight(totalLength, ' ');
        }

        public string PadRight(int totalLength, char paddingSymbol)
        {
            if (totalLength <= Length)
                return this;
            var result = new char[totalLength];
            int index = 0;
            for (var i = 0; i < Length; i++)
            {
                result[index++] = this[i];
            }
            for (; index < totalLength; index++)
            {
                result[index] = paddingSymbol;
            }
            return new string(result);
        }

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
            for (int i = Length-1; i >= 0; i--)
                if (!InSet(this[i], charSet))
                    return i;
            return -1;
        }

        bool InSet(char c, char[] charSet)
        {
            for (int i = 0; i < charSet.Length; i++) if (charSet[i] == c) return true;
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
            for (int i = Length-1; i >= 0; i--)
                if (!char.IsWhiteSpace(this[i]))
                    return i;
            return -1;
        }

        public string Insert(int pos, string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (pos < 0 || pos > this.Length)
                throw new ArgumentOutOfRangeException(nameof(pos));

            if (str.Length == 0)
                return this;
            if (this.Length == 0)
                return str;

            var s = new char[this.Length + str.Length];
            for (int i = 0; i < pos; i++)
                s[i] = this[i];
            for (int i = 0; i < str.Length; i++)
                s[i+pos] = str[i];
            for (int i = pos; i < this.Length; i++)
                s[i+str.Length] = this[i];
            return new string(s);
        }

        public int IndexOf(string str, int startIndex=0)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (str == Empty)
                return 0;

            if (startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            for (int hay=startIndex; hay < Length; hay++)
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
            for (int i = 0; i < Math.Min(a.Length, b.Length); i++)
            {
                if (a[i] < b[i]) return -1;
                if (b[i] < a[i]) return 1;
            }
            if (a.Length < b.Length) return -1;
            if (b.Length < a.Length) return 1;
            return 0;
        }

        [DotNetOverride]
        public static string Format(string str, params object[] objs)
        {
            var builder = new StringBuilder();
            var tokens = FormatStringTokenizer.TokenizeFormatString(str);
            foreach (var token in tokens)
            {
                builder.Append(token.ToString(objs));
            }
            return builder.ToString();
        }

        public bool Contains(string str)
        {
            return IndexOf(str, 0) >= 0;
        }
    }
}
