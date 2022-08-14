using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Double")]
    [extern(CPLUSPLUS) Set("TypeName", "double")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    /** Represents a double-precision floating-point number. */
    public intrinsic struct Double
    {
        public const double MinValue = -1.7976931348623157e308;
        public const double MaxValue = 1.7976931348623157e308;
        public const double NaN = .0/.0;
        public const double PositiveInfinity = 1./.0;
        public const double NegativeInfinity = -1./.0;

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            if defined(CPLUSPLUS)
            @{
                uint32_t data[2];
                memcpy(data, $$, sizeof(data));

                int hash = 27;
                hash = (13 * hash) + data[0];
                hash = (13 * hash) + data[1];
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
                char buf[64];
                int len = snprintf(buf, sizeof(buf), "%f", *$$);
                if (len < 0 && errno == ERANGE)
                {
                    // Some snprintf implementations return -1 and sets errno to
                    // ERANGE instead of returning the desired length, so let's
                    // reconstruct the value we want here.
                    len = snprintf(nullptr, 0, "%f", *$$);
                    U_ASSERT(len > sizeof(buf));
                }

                char* ptr = buf;
                if (len > sizeof(buf))
                {
                    // Stackalloc bigger buffer, and try again
                    ptr = (char*)alloca(len + 1);
                    len = snprintf(ptr, len + 1, "%f", *$$);
                }

                // Trim .0 ending
                while (len > 1 && ptr[len - 1] == '0')
                    len--;
                if (len > 1 && ptr[len - 1] == '.')
                    len--;

                U_ASSERT(len >= 0);
                return uString::Ansi(ptr, len);
            @}
            else
                return base.ToString();
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{FormatException:Include}")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{OverflowException:Include}")]
        public static double Parse(string str)
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
                double retval = strtod(trimmed, &end);
                while (*end && isspace(*end))
                    end++;

                if (errno == ERANGE)
                    U_THROW(@{OverflowException(string):New(uString::Const("Value was either too large or too small for double"))});

                if (!strlen(trimmed) || strlen(end))
                    U_THROW(@{FormatException(string):New(uString::Const("Unable to convert string to double"))});

                return retval;
            @}
            else
                build_error;
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        public static bool TryParse(string str, out double result)
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
                *$1 = strtod(trimmed, &end);
                while (*end && isspace(*end))
                    end++;

                if (errno == ERANGE || !strlen(trimmed) || strlen(end))
                {
                    *$1 = 0;
                    return false;
                }

                return true;
            @}
            else
                build_error;
        }

        public static intrinsic double operator - (double a);

        public static intrinsic double operator + (double a, double b);
        public static intrinsic double operator - (double a, double b);
        public static intrinsic double operator * (double a, double b);
        public static intrinsic double operator / (double a, double b);

        public static intrinsic bool operator < (double a, double b);
        public static intrinsic bool operator > (double a, double b);
        public static intrinsic bool operator <= (double a, double b);
        public static intrinsic bool operator >= (double a, double b);

        public static intrinsic bool operator == (double left, double right);
        public static intrinsic bool operator != (double left, double right);

        public static intrinsic implicit operator double(sbyte v);
        public static intrinsic implicit operator double(byte v);
        public static intrinsic implicit operator double(short v);
        public static intrinsic implicit operator double(ushort v);
        public static intrinsic implicit operator double(char v);
        public static intrinsic implicit operator double(int v);
        public static intrinsic implicit operator double(uint v);
        public static intrinsic implicit operator double(long v);
        public static intrinsic implicit operator double(ulong v);
        public static intrinsic implicit operator double(float v);

        public static bool IsNaN(double d)
        {
            return d != d;
        }

        public static bool IsNegativeInfinity(double d)
        {
            return d == NegativeInfinity;
        }

        public static bool IsPositiveInfinity(double d)
        {
            return d == PositiveInfinity;
        }

        public static bool IsInfinity(double d)
        {
            return IsNegativeInfinity(d) || IsPositiveInfinity(d);
        }
    }
}
