using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Byte")]
    [extern(CPLUSPLUS) Set("TypeName", "uint8_t")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    /** Represents an 8-bit unsigned integer.
        The value can only be in the range 0 to 255. Storage size is 1 byte. */
    public intrinsic struct Byte
    {
        public const byte MinValue = 0;
        public const byte MaxValue = 0xff;

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
                char buf[4];
                int len = snprintf(buf, sizeof(buf), "%d", *$$);
                return uString::Ansi(buf, len);
            @}
            else
                return base.ToString();
        }

        public static intrinsic int operator - (byte a);
        public static intrinsic int operator + (byte a, byte b);
        public static intrinsic int operator - (byte a, byte b);
        public static intrinsic int operator * (byte a, byte b);
        public static intrinsic int operator / (byte a, byte b);
        public static intrinsic int operator % (byte a, byte b);

        public static intrinsic byte operator ~ (byte a);
        public static intrinsic byte operator | (byte a, byte b);
        public static intrinsic byte operator & (byte a, byte b);
        public static intrinsic byte operator ^ (byte a, byte b);

        public static intrinsic int operator << (byte a, int b);
        public static intrinsic int operator >> (byte a, int b);

        public static intrinsic bool operator < (byte a, byte b);
        public static intrinsic bool operator > (byte a, byte b);
        public static intrinsic bool operator <= (byte a, byte b);
        public static intrinsic bool operator >= (byte a, byte b);

        public static intrinsic bool operator == (byte left, byte right);
        public static intrinsic bool operator != (byte left, byte right);

        public static intrinsic explicit operator byte(sbyte v);
        public static intrinsic explicit operator byte(short v);
        public static intrinsic explicit operator byte(ushort v);
        public static intrinsic explicit operator byte(char v);
        public static intrinsic explicit operator byte(int v);
        public static intrinsic explicit operator byte(uint v);
        public static intrinsic explicit operator byte(long v);
        public static intrinsic explicit operator byte(ulong v);
        public static intrinsic explicit operator byte(float v);
        public static intrinsic explicit operator byte(double v);

        private const string _parseOverflowExceptionMessage = "Value was either too large or too small for an unsigned byte";
        private const string _parseFormatExceptionMessage = "Unable to convert string to unsigned byte";

        [TargetSpecificImplementation]
        public static byte Parse(string str)
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

            return (byte)parsedIntValue;
        }

        [TargetSpecificImplementation]
        public static bool TryParse(string str, out byte res)
        {
            res = default(byte);
            int parsedIntValue;
            if (int.TryParse(str, out parsedIntValue))
            {
                if (parsedIntValue <= MaxValue && parsedIntValue >= MinValue)
                {
                    res = (byte)parsedIntValue;
                    return true;
                }
            }
            return false;
        }
    }
}
