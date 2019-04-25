using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Int64")]
    [extern(CPLUSPLUS) Set("TypeName", "int64_t")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    /** Represents a 64-bit signed integer. */
    public intrinsic struct Long
    {
        public const long MinValue = -(long)(0x8000000000000000 - 1) - 1;
        public const long MaxValue = 0x7fffffffffffffff;

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            if defined(CPLUSPLUS)
            @{
                int hash = 27;
                hash = (13 * hash) + (*$$ & UINT32_MAX);
                hash = (13 * hash) + (*$$ >> 32);
                return hash;
            @}
            else
                return base.GetHashCode();
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cstdio")]
        public override string ToString()
        {
            if defined(CPLUSPLUS)
            @{
                char buf[21];
                int len = snprintf(buf, sizeof(buf), "%" PRId64, *$$);
                return uString::Ansi(buf, len);
            @}
            else
                return base.ToString();
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{FormatException:Include}")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{OverflowException:Include}")]
        public static long Parse(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if defined(CPLUSPLUS)
            @{
                errno = 0;
                uCString cstr($0);
                const char* trimmed = cstr.Ptr;
                while (*trimmed && isspace(*trimmed))
                    trimmed++;
                char* end;
                long long retval = strtoll(trimmed, &end, 10);
                while (*end && isspace(*end))
                    end++;

                if (errno == ERANGE)
                    U_THROW(@{OverflowException(string):New(uString::Const("Value was either too large or too small for long"))});

                if (!strlen(trimmed) || strlen(end))
                    U_THROW(@{FormatException(string):New(uString::Const("Unable to convert string to long"))});

                return (int64_t)retval;
            @}
            else
                build_error;
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        public static bool TryParse(string str, out long result)
        {
            if (str == null)
            {
                result = 0;
                return false;
            }

            if defined(CPLUSPLUS)
            @{
                errno = 0;
                uCString cstr($0);
                const char* trimmed = cstr.Ptr;
                while (*trimmed && isspace(*trimmed))
                    trimmed++;
                char* end;
                long long retval = strtoll(trimmed, &end, 10);
                while (*end && isspace(*end))
                    end++;

                if (errno == ERANGE || !strlen(trimmed) || strlen(end))
                {
                    *$1 = 0;
                    return false;
                }

                *$1 = (int64_t)retval;
                return true;
            @}
            else
                build_error;
        }

        public static intrinsic long operator - (long a);
        public static intrinsic long operator ~ (long a);

        public static intrinsic long operator + (long a, long b);
        public static intrinsic long operator - (long a, long b);
        public static intrinsic long operator * (long a, long b);
        public static intrinsic long operator / (long a, long b);
        public static intrinsic long operator % (long a, long b);

        public static intrinsic long operator | (long a, long b);
        public static intrinsic long operator & (long a, long b);
        public static intrinsic long operator ^ (long a, long b);

        public static intrinsic long operator << (long a, int b);
        public static intrinsic long operator >> (long a, int b);

        public static intrinsic bool operator < (long a, long b);
        public static intrinsic bool operator > (long a, long b);
        public static intrinsic bool operator <= (long a, long b);
        public static intrinsic bool operator >= (long a, long b);

        public static intrinsic bool operator == (long left, long right);
        public static intrinsic bool operator != (long left, long right);

        public static intrinsic implicit operator long(sbyte v);
        public static intrinsic implicit operator long(byte v);
        public static intrinsic implicit operator long(short v);
        public static intrinsic implicit operator long(ushort v);
        public static intrinsic implicit operator long(char v);
        public static intrinsic implicit operator long(int v);
        public static intrinsic implicit operator long(uint v);
        public static intrinsic explicit operator long(ulong v);
        public static intrinsic explicit operator long(float v);
        public static intrinsic explicit operator long(double v);
    }
}
