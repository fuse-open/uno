using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Int32")]
    [extern(CPLUSPLUS) Set("TypeName", "int32_t")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    /** Represents a 32-bit signed integer. */
    public intrinsic struct Int
    {
        public const int MinValue = (int)-0x80000000;
        public const int MaxValue = 0x7fffffff;

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            if defined(CPLUSPLUS)
            @{
                return *$$;
            @}
            else
                return base.GetHashCode();
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cstdio")]
        public override string ToString()
        {
            if defined(CPLUSPLUS)
            @{
                char buf[12];
                int len = snprintf(buf, sizeof(buf), "%d", *$$);
                return uString::Ansi(buf, len);
            @}
            else
                return base.ToString();
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "climits")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{FormatException:Include}")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{OverflowException:Include}")]
        public static int Parse(string str)
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
                long retval = strtol(trimmed, &end, 10);
                while (*end && isspace(*end))
                    end++;

                if (errno == ERANGE || retval > INT_MAX || retval < INT_MIN)
                    U_THROW(@{OverflowException(string):New(uString::Const("Value was either too large or too small for int"))});

                if (!strlen(trimmed) || strlen(end))
                    U_THROW(@{FormatException(string):New(uString::Const("Unable to convert string to int"))});

                return (int)retval;
            @}
            else
                build_error;
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "climits")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        public static bool TryParse(string str, out int result)
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
                long retval = strtol(trimmed, &end, 10);
                while (*end && isspace(*end))
                    end++;

                if (errno == ERANGE || !strlen(trimmed) || strlen(end) ||
                        retval > INT_MAX || retval < INT_MIN)
                {
                    *$1 = 0;
                    return false;
                }

                *$1 = (int)retval;
                return true;
            @}
            else
                build_error;
        }

        public static intrinsic int operator - (int a);
        public static intrinsic int operator ~ (int a);

        public static intrinsic int operator + (int a, int b);
        public static intrinsic int operator - (int a, int b);
        public static intrinsic int operator * (int a, int b);
        public static intrinsic int operator / (int a, int b);
        public static intrinsic int operator % (int a, int b);

        public static intrinsic int operator | (int a, int b);
        public static intrinsic int operator & (int a, int b);
        public static intrinsic int operator ^ (int a, int b);

        public static intrinsic int operator << (int a, int b);
        public static intrinsic int operator >> (int a, int b);

        public static intrinsic bool operator < (int a, int b);
        public static intrinsic bool operator > (int a, int b);
        public static intrinsic bool operator <= (int a, int b);
        public static intrinsic bool operator >= (int a, int b);

        public static intrinsic bool operator == (int left, int right);
        public static intrinsic bool operator != (int left, int right);

        public static intrinsic implicit operator int(sbyte v);
        public static intrinsic implicit operator int(byte v);
        public static intrinsic implicit operator int(short v);
        public static intrinsic implicit operator int(ushort v);
        public static intrinsic implicit operator int(char v);
        public static intrinsic explicit operator int(uint v);
        public static intrinsic explicit operator int(long v);
        public static intrinsic explicit operator int(ulong v);
        public static intrinsic explicit operator int(float v);
        public static intrinsic explicit operator int(double v);
    }
}
