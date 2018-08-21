using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.UInt32")]
    [extern(CPLUSPLUS) Set("TypeName", "uint32_t")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    public intrinsic struct UInt
    {
        public const uint MinValue = 0;
        public const uint MaxValue = 0xffffffff;

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
                char buf[11];
                int len = snprintf(buf, sizeof(buf), "%u", *$$);
                return uString::Ansi(buf, len);
            @}
            else
                return base.ToString();
        }

        public static intrinsic long operator - (uint a);
        public static intrinsic uint operator + (uint a, uint b);
        public static intrinsic uint operator - (uint a, uint b);
        public static intrinsic uint operator * (uint a, uint b);
        public static intrinsic uint operator / (uint a, uint b);
        public static intrinsic uint operator % (uint a, uint b);

        public static intrinsic uint operator ~ (uint a);
        public static intrinsic uint operator | (uint a, uint b);
        public static intrinsic uint operator & (uint a, uint b);
        public static intrinsic uint operator ^ (uint a, uint b);

        public static intrinsic uint operator << (uint a, int b);
        public static intrinsic uint operator >> (uint a, int b);

        public static intrinsic bool operator < (uint a, uint b);
        public static intrinsic bool operator > (uint a, uint b);
        public static intrinsic bool operator <= (uint a, uint b);
        public static intrinsic bool operator >= (uint a, uint b);
        public static intrinsic bool operator == (uint left, uint right);
        public static intrinsic bool operator != (uint left, uint right);

        public static intrinsic explicit operator uint(sbyte v);
        public static intrinsic implicit operator uint(byte v);
        public static intrinsic explicit operator uint(short v);
        public static intrinsic implicit operator uint(ushort v);
        public static intrinsic implicit operator uint(char v);
        public static intrinsic explicit operator uint(int v);
        public static intrinsic explicit operator uint(long v);
        public static intrinsic explicit operator uint(ulong v);
        public static intrinsic explicit operator uint(float v);
        public static intrinsic explicit operator uint(double v);

        private const string _parseOverflowExceptionMessage = "Value was either too large or too small for unsigned int";
        private const string _parseFormatExceptionMessage = "Unable to convert string to unsigned int";

        public static uint Parse(string str)
        {
            var parsedLongValue = 0L;
            try
            {
                parsedLongValue = long.Parse(str);
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

            if (parsedLongValue < (long)MinValue || parsedLongValue > (long)MaxValue)
            {
                throw new OverflowException(_parseOverflowExceptionMessage);
            }

            return (uint)parsedLongValue;
        }

        public static bool TryParse(string str, out uint res)
        {
            res = default(uint);
            long parsedLongValue;
            if (long.TryParse(str, out parsedLongValue))
            {
                if (parsedLongValue <= (long)MaxValue && parsedLongValue >= (long)MinValue)
                {
                    res = (uint)parsedLongValue;
                    return true;
                }
            }
            return false;
        }
    }
}
