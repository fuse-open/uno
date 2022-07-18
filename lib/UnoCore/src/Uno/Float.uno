using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Single")]
    [extern(CPLUSPLUS) Set("TypeName", "float")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    /** Represents a single-precision floating-point number. */
    public intrinsic struct Float
    {
        public const float MinValue = -3.402823e38f;
        public const float MaxValue = 3.402823e38f;
        public const float NaN = .0f/.0f;
        public const float PositiveInfinity = 1.f/.0f;
        public const float NegativeInfinity = -1.f/.0f;

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            if defined(CPLUSPLUS)
            @{
                union {
                    float f;
                    int i;
                } u;
                u.f = *$$;
                return u.i;
            @}
            else
                return base.GetHashCode();
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cstdio")]
        public override string ToString()
        {
            if defined(CPLUSPLUS)
                return ((double) this).ToString();
            else
                return base.ToString();
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{FormatException:Include}")]
        [extern(CPLUSPLUS) Require("Source.Include", "@{OverflowException:Include}")]
        public static float Parse(string str)
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

                if (errno == ERANGE || retval > @{MaxValue} || retval < @{MinValue})
                    U_THROW(@{OverflowException(string):New(uString::Const("Value was either too large or too small for float"))});

                if (!strlen(trimmed) || strlen(end))
                    U_THROW(@{FormatException(string):New(uString::Const("Unable to convert string to float"))});

                return (float)retval;
            @}
            else
                build_error;
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cctype")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        public static bool TryParse(string str, out float result)
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
                double retval = strtod(trimmed, &end);
                while (*end && isspace(*end))
                    end++;

                if (errno == ERANGE || !strlen(trimmed) || strlen(end) ||
                        retval > @{MaxValue} || retval < @{MinValue})
                {
                    *$1 = 0;
                    return false;
                }

                *$1 = (float)retval;
                return true;
            @}
            else
                build_error;
        }

        public static intrinsic float operator - (float a);

        public static intrinsic float operator + (float a, float b);
        public static intrinsic float operator - (float a, float b);
        public static intrinsic float operator * (float a, float b);
        public static intrinsic float operator / (float a, float b);

        public static intrinsic bool operator < (float a, float b);
        public static intrinsic bool operator > (float a, float b);
        public static intrinsic bool operator <= (float a, float b);
        public static intrinsic bool operator >= (float a, float b);

        public static intrinsic bool operator == (float left, float right);
        public static intrinsic bool operator != (float left, float right);

        public static intrinsic implicit operator float(sbyte v);
        public static intrinsic implicit operator float(byte v);
        public static intrinsic implicit operator float(short v);
        public static intrinsic implicit operator float(ushort v);
        public static intrinsic implicit operator float(char v);
        public static intrinsic implicit operator float(int v);
        public static intrinsic implicit operator float(uint v);
        public static intrinsic implicit operator float(long v);
        public static intrinsic implicit operator float(ulong v);
        public static intrinsic explicit operator float(double v);

        public static bool IsNaN(float f)
        {
            return f != f;
        }

        public static bool IsNegativeInfinity(float f)
        {
            return f == NegativeInfinity;
        }

        public static bool IsPositiveInfinity(float f)
        {
            return f == PositiveInfinity;
        }

        public static bool IsInfinity(float f)
        {
            return IsNegativeInfinity(f) || IsPositiveInfinity(f);
        }
    }
}
