using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.UInt16")]
    [extern(CPLUSPLUS) Set("TypeName", "uint16_t")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    public intrinsic struct UShort
    {
        public const ushort MinValue = 0;
        public const ushort MaxValue = 0xffff;

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
                char buf[6];
                int len = snprintf(buf, sizeof(buf), "%u", *$$);
                return uString::Ansi(buf, len);
            @}
            else
                return base.ToString();
        }

        public static intrinsic int operator - (ushort a);
        public static intrinsic int operator + (ushort a, ushort b);
        public static intrinsic int operator - (ushort a, ushort b);
        public static intrinsic int operator * (ushort a, ushort b);
        public static intrinsic int operator / (ushort a, ushort b);
        public static intrinsic int operator % (ushort a, ushort b);

        public static intrinsic ushort operator ~ (ushort a);
        public static intrinsic ushort operator | (ushort a, ushort b);
        public static intrinsic ushort operator & (ushort a, ushort b);
        public static intrinsic ushort operator ^ (ushort a, ushort b);

        public static intrinsic int operator << (ushort a, int b);
        public static intrinsic int operator >> (ushort a, int b);

        public static intrinsic bool operator < (ushort a, ushort b);
        public static intrinsic bool operator > (ushort a, ushort b);
        public static intrinsic bool operator <= (ushort a, ushort b);
        public static intrinsic bool operator >= (ushort a, ushort b);
        public static intrinsic bool operator == (ushort left, ushort right);
        public static intrinsic bool operator != (ushort left, ushort right);

        public static intrinsic explicit operator ushort(sbyte v);
        public static intrinsic implicit operator ushort(byte v);
        public static intrinsic explicit operator ushort(short v);
        public static intrinsic implicit operator ushort(char v);
        public static intrinsic explicit operator ushort(int v);
        public static intrinsic explicit operator ushort(uint v);
        public static intrinsic explicit operator ushort(long v);
        public static intrinsic explicit operator ushort(ulong v);
        public static intrinsic explicit operator ushort(float v);
        public static intrinsic explicit operator ushort(double v);

        private const string _parseOverflowExceptionMessage = "Value was either too large or too small for unsigned short";
        private const string _parseFormatExceptionMessage = "Unable to convert string to unsigned short";

        public static ushort Parse(string str)
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

            return (ushort)parsedIntValue;
        }

        public static bool TryParse(string str, out ushort res)
        {
            res = default(ushort);
            int parsedIntValue;
            if (int.TryParse(str, out parsedIntValue))
            {
                if (parsedIntValue <= MaxValue && parsedIntValue >= MinValue)
                {
                    res = (ushort)parsedIntValue;
                    return true;
                }
            }
            return false;
        }
    }
}
