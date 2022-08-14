using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Int16")]
    [extern(CPLUSPLUS) Set("TypeName", "int16_t")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    /** Represents a 16-bit signed integer. */
    public intrinsic struct Short
    {
        public const short MinValue = -0x8000;
        public const short MaxValue = 0x7fff;

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            if defined(CPLUSPLUS)
            @{
                return (int)*$$;
            @}
            else
                return base.GetHashCode();
        }

        [extern(CPLUSPLUS) Require("Source.Include", "cstdio")]
        public override string ToString()
        {
            if defined(CPLUSPLUS)
            @{
                char buf[7];
                int len = snprintf(buf, sizeof(buf), "%d", *$$);
                return uString::Ansi(buf, len);
            @}
            else
                return base.ToString();
        }

        public static intrinsic int operator - (short a);
        public static intrinsic int operator + (short a, short b);
        public static intrinsic int operator - (short a, short b);
        public static intrinsic int operator * (short a, short b);
        public static intrinsic int operator / (short a, short b);
        public static intrinsic int operator % (short a, short b);

        public static intrinsic short operator ~ (short a);
        public static intrinsic short operator | (short a, short b);
        public static intrinsic short operator & (short a, short b);
        public static intrinsic short operator ^ (short a, short b);

        public static intrinsic int operator << (short a, int b);
        public static intrinsic int operator >> (short a, int b);

        public static intrinsic bool operator < (short a, short b);
        public static intrinsic bool operator > (short a, short b);
        public static intrinsic bool operator <= (short a, short b);
        public static intrinsic bool operator >= (short a, short b);
        public static intrinsic bool operator == (short left, short right);
        public static intrinsic bool operator != (short left, short right);

        public static intrinsic implicit operator short(sbyte v);
        public static intrinsic implicit operator short(byte v);
        public static intrinsic explicit operator short(ushort v);
        public static intrinsic explicit operator short(char v);
        public static intrinsic explicit operator short(int v);
        public static intrinsic explicit operator short(uint v);
        public static intrinsic explicit operator short(long v);
        public static intrinsic explicit operator short(ulong v);
        public static intrinsic explicit operator short(float v);
        public static intrinsic explicit operator short(double v);

        private const string _parseOverflowExceptionMessage = "Value was either too large or too small for short";
        private const string _parseFormatExceptionMessage = "Unable to convert string to short";

        public static short Parse(string str)
        {
            var parsedIntValue = 0;
            try
            {
                parsedIntValue = int.Parse(str);
            }
            catch(OverflowException ex)
            {
                //re-throw exception with custom message;
                throw new OverflowException(_parseOverflowExceptionMessage);
            }
            catch(FormatException ex)
            {
                //re-throw exception with custom message;
                throw new FormatException(_parseFormatExceptionMessage);
            }

            if (parsedIntValue < MinValue || parsedIntValue > MaxValue)
            {
                throw new OverflowException(_parseOverflowExceptionMessage);
            }

            return (short)parsedIntValue;
        }

        public static bool TryParse(string str, out short res)
        {
            res = default(short);
            int parsedIntValue;
            if (int.TryParse(str, out parsedIntValue))
            {
                if (parsedIntValue <= MaxValue && parsedIntValue >= MinValue)
                {
                    res = (short)parsedIntValue;
                    return true;
                }
            }
            return false;
        }
    }
}
