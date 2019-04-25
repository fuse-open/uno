using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.UInt64")]
    [extern(CPLUSPLUS) Set("TypeName", "uint64_t")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    public intrinsic struct ULong
    {
        public const ulong MinValue = 0;
        public const ulong MaxValue = 0xffffffffffffffff;

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
                int len = snprintf(buf, sizeof(buf), "%" PRIu64, *$$);
                return uString::Ansi(buf, len);
            @}
            else
                return base.ToString();
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{FormatException:Include}")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{OverflowException:Include}")]
        public static ulong Parse(string str)
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
                unsigned long long retval = strtoull(trimmed, &end, 10);
                while (*end && isspace(*end))
                    end++;

                if (strchr(trimmed, '-'))
                    U_THROW(@{OverflowException(string):New(uString::Const("Value was either too large or too small for ulong"))});

                if (!strlen(trimmed) || strlen(end))
                    U_THROW(@{FormatException(string):New(uString::Const("Unable to convert string to ulong"))});

                return (uint64_t)retval;
            @}
            else
                build_error;
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        public static bool TryParse(string str, out ulong result)
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
                unsigned long long retval = strtoull(trimmed, &end, 10);
                while (*end && isspace(*end))
                    end++;

                if (errno == ERANGE || !strlen(trimmed) || strlen(end) ||
                        strchr(trimmed, '-'))
                {
                    *$1 = 0;
                    return false;
                }

                *$1 = (uint64_t)retval;
                return true;
            @}
            else
                build_error;
        }

        public static intrinsic ulong operator ~ (ulong a);

        public static intrinsic ulong operator + (ulong a, ulong b);
        public static intrinsic ulong operator - (ulong a, ulong b);
        public static intrinsic ulong operator * (ulong a, ulong b);
        public static intrinsic ulong operator / (ulong a, ulong b);
        public static intrinsic ulong operator % (ulong a, ulong b);

        public static intrinsic ulong operator | (ulong a, ulong b);
        public static intrinsic ulong operator & (ulong a, ulong b);
        public static intrinsic ulong operator ^ (ulong a, ulong b);

        public static intrinsic ulong operator << (ulong a, int b);
        public static intrinsic ulong operator >> (ulong a, int b);

        public static intrinsic bool operator < (ulong a, ulong b);
        public static intrinsic bool operator > (ulong a, ulong b);
        public static intrinsic bool operator <= (ulong a, ulong b);
        public static intrinsic bool operator >= (ulong a, ulong b);

        public static intrinsic bool operator == (ulong left, ulong right);
        public static intrinsic bool operator != (ulong left, ulong right);

        public static intrinsic explicit operator ulong(sbyte v);
        public static intrinsic implicit operator ulong(byte v);
        public static intrinsic explicit operator ulong(short v);
        public static intrinsic implicit operator ulong(ushort v);
        public static intrinsic implicit operator ulong(char v);
        public static intrinsic explicit operator ulong(int v);
        public static intrinsic implicit operator ulong(uint v);
        public static intrinsic explicit operator ulong(long v);
        public static intrinsic explicit operator ulong(float v);
        public static intrinsic explicit operator ulong(double v);
    }
}
