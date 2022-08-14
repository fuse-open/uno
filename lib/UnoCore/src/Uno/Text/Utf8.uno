using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Text
{
    [extern(DOTNET) DotNetType("System.Text.Decoder")]
    public abstract class Decoder
    {
        public abstract int GetCharCount(byte[] bytes, int index, int count);
        public abstract int GetChars(byte[] bytes, int byteIndex, int byteCount,
                                     char[] chars, int charIndex);
    }

    sealed class UTF8Decoder : Decoder
    {
        int _state;

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (bytes.Length - index < count)
                throw new ArgumentOutOfRangeException(nameof(bytes));

            return ConvertInternal(bytes, index, count, null, 0, 0);
        }

        const int StopAction   = 1 << 28;
        const int Check2Action = 2 << 28;
        const int Check3Action = 3 << 28;
        const int ActionMask   = 3 << 28;

        int ConvertInternal(byte[] bytes, int byteIndex, int byteCount,
                            char[] chars, int charIndex, int charCount)
        {
            int bytesUsed = 0;
            int charsUsed = 0;

            int ch = _state;
            _state = 0;
            while (bytesUsed < byteCount)
            {
                if (ch == 0)
                {
                    ch = bytes[byteIndex + bytesUsed++];

                    if (ch < 128)
                    {
                        // ASCII (common case)
                        if (chars != null)
                        {
                            if (charsUsed >= charCount)
                                throw new ArgumentException(nameof(chars));

                            chars[charIndex + charsUsed] = (char)ch;
                        }

                        ++charsUsed;
                        ch = 0;
                        continue;
                    }

                    // decode leading byte
                    if ((ch & 0xE0) == 0xC0)
                    {
                        ch &= 0x3F >> 1;

                        if (ch <= 1)
                        {
                            if (chars != null)
                            {
                                if (charsUsed >= charCount)
                                    throw new ArgumentException(nameof(chars));

                                chars[charIndex + charsUsed] = '\uFFFD';
                            }

                            ++charsUsed;
                            ch = 0;
                            continue;
                        }

                        ch |= StopAction >> 6;
                    }
                    else if ((ch & 0xF0) == 0xE0)
                    {
                        ch &= 0x3F >> 2;
                        ch |= Check2Action >> 6 | StopAction >> 6 * 2;
                    }
                    else if ((ch & 0xF8) == 0xF0)
                    {
                        ch &= 0x3F >> 3;

                        if (ch > 0x04)
                        {
                            if (chars != null)
                            {
                                if (charsUsed >= charCount)
                                    throw new ArgumentException(nameof(chars));

                                chars[charIndex + charsUsed] = '\uFFFD';
                            }

                            ++charsUsed;
                            ch = 0;
                            continue;
                        }

                        ch |= Check3Action >> 6 | StopAction >> 6 * 3;
                    }
                    else
                    {
                        if (chars != null)
                        {
                            if (charsUsed >= charCount)
                                throw new ArgumentException(nameof(chars));

                            // invalid encoding
                            chars[charIndex + charsUsed] = '\uFFFD';
                        }

                        ++charsUsed;
                        ch = 0;
                        continue;
                    }
                }

                bool done = false, invalid = false;
                do
                {
                    if (bytesUsed == byteCount)
                    {
                        _state = ch;
                        return charsUsed;
                    }

                    byte ch2 = bytes[byteIndex + bytesUsed]; // peek

                    // decode trailing byte
                    if ((ch2 & 0xC0) != 0x80)
                    {
                        invalid = true;
                        break;
                    }

                    // update state
                    ch = ch << 6 | ch2 & 0x3F;
                    ++bytesUsed;

                    switch (ch & ActionMask)
                    {
                    case StopAction:
                        done = true;
                        break;

                    case Check2Action:
                        if ((ch & 0x1F << 5) == 0 ||
                            (ch & 0xF800 >> 6) == 0xD800 >> 6)
                        {
                            invalid = true;
                            done = true;
                        }
                        break;

                    case Check3Action:
                        if ((ch & 0x1F0) < 0x10 || (ch & 0x1F0) > 0x100)
                        {
                            invalid = true;
                            done = true;
                        }
                        break;
                    }
                }
                while (!done);

                if (invalid)
                {
                    if (chars != null)
                    {
                        if (charsUsed >= charCount)
                            throw new ArgumentException(nameof(chars));

                        chars[charIndex + charsUsed] = '\uFFFD';
                    }

                    ++charsUsed;
                }
                else
                {
                    int codePoint = ch & 0x1FFFFF;

                    if (codePoint < 0x10000)
                    {
                        if (chars != null)
                        {
                            if (charsUsed >= charCount)
                                throw new ArgumentException(nameof(chars));

                            // output single UTF-16 code-point
                            chars[charIndex + charsUsed] = (char)codePoint;
                        }

                        ++charsUsed;
                    }
                    else
                    {
                        if (chars != null)
                        {
                            if (charsUsed + 1 >= charCount)
                                throw new ArgumentException(nameof(chars));

                            // output surrogate pair
                            codePoint -= 0x10000;
                            chars[charIndex + charsUsed] = (char)(0xD800 + (codePoint >> 10));
                            chars[charIndex + charsUsed + 1] = (char)(0xDC00 + (codePoint & 0x3FF));
                        }

                        charsUsed += 2;
                    }
                }

                ch = 0; // make sure we read a new byte next
            }

            _state = 0;
            return charsUsed;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount,
                                     char[] chars, int charIndex)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (byteIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(byteIndex));
            if (byteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(byteCount));
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));
            if (charIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(charIndex));

            if (bytes.Length - byteIndex < byteCount)
                throw new ArgumentOutOfRangeException(nameof(bytes));

            int charCount = chars.Length - charIndex;
            return ConvertInternal(bytes, byteIndex, byteCount,
                                   chars, charIndex, charCount);
        }
    }

    [extern(DOTNET) DotNetType("System.Text.Encoding")]
    public abstract class Encoding
    {
        extern(DOTNET)
        public static Encoding ASCII { get; }

        static internal UTF8Encoding _utf8;
        public static Encoding UTF8
        {
            get
            {
                if (_utf8 == null)
                    _utf8 = new UTF8Encoding();

                return _utf8;
            }
        }

        public abstract Decoder GetDecoder();

        extern(DOTNET)
        public virtual byte[] GetBytes(string s);
        extern(DOTNET)
        public virtual string GetString(byte[] bytes);
        extern(DOTNET)
        public virtual string GetString(byte[] bytes, int index, int count);
    }

    [extern(DOTNET) DotNetType("System.Text.UTF8Encoding")]
    public class UTF8Encoding : Encoding
    {
        public override Decoder GetDecoder()
        {
            return new UTF8Decoder();
        }
    }

    public static class Utf8
    {
        public static byte[] GetBytes(string value)
        {
            if defined(DOTNET)
                return Encoding.UTF8.GetBytes(value);
            else
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                @{
                    uCString cstr($0);
                    return uArray::New(@{byte[]:TypeOf}, cstr.Length, cstr.Ptr);
                @}
            }
        }

        public static string GetString(byte[] value)
        {
            if defined(DOTNET)
                return Encoding.UTF8.GetString(value);
            else
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                @{
                    const char* utf8 = (const char*)uPtr($0)->Ptr();
                    return uString::Utf8(utf8, $0->Length());
                @}
            }
        }

        public static string GetString(byte[] value, int index, int count)
        {
            if defined(DOTNET)
                return Encoding.UTF8.GetString(value, index, count);
            else
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (index < 0 || index >= value.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (count < 0 || index + count > value.Length)
                    throw new ArgumentOutOfRangeException(nameof(count));

                @{
                    const char* utf8 = (const char*)uPtr($0)->Ptr();
                    return uString::Utf8(utf8 + index, count);
                @}
            }
        }
    }
}
