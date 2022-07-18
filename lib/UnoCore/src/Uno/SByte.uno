using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.SByte")]
    [extern(CPLUSPLUS) Set("TypeName", "int8_t")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    /** Represents an 8-bit signed integer. */
    public intrinsic struct SByte
    {
        public const sbyte MinValue = -0x80;
        public const sbyte MaxValue = 0x7f;

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
                char buf[5];
                int len = snprintf(buf, sizeof(buf), "%d", *$$);
                return uString::Ansi(buf, len);
            @}
            else
                return base.ToString();
        }

        public static intrinsic int operator - (sbyte a);
        public static intrinsic int operator + (sbyte a, sbyte b);
        public static intrinsic int operator - (sbyte a, sbyte b);
        public static intrinsic int operator * (sbyte a, sbyte b);
        public static intrinsic int operator / (sbyte a, sbyte b);
        public static intrinsic int operator % (sbyte a, sbyte b);

        public static intrinsic sbyte operator ~ (sbyte a);
        public static intrinsic sbyte operator | (sbyte a, sbyte b);
        public static intrinsic sbyte operator & (sbyte a, sbyte b);
        public static intrinsic sbyte operator ^ (sbyte a, sbyte b);

        public static intrinsic int operator << (sbyte a, int b);
        public static intrinsic int operator >> (sbyte a, int b);

        public static intrinsic bool operator < (sbyte a, sbyte b);
        public static intrinsic bool operator > (sbyte a, sbyte b);
        public static intrinsic bool operator <= (sbyte a, sbyte b);
        public static intrinsic bool operator >= (sbyte a, sbyte b);
        public static intrinsic bool operator == (sbyte left, sbyte right);
        public static intrinsic bool operator != (sbyte left, sbyte right);

        public static intrinsic explicit operator sbyte(byte v);
        public static intrinsic explicit operator sbyte(short v);
        public static intrinsic explicit operator sbyte(ushort v);
        public static intrinsic explicit operator sbyte(char v);
        public static intrinsic explicit operator sbyte(int v);
        public static intrinsic explicit operator sbyte(uint v);
        public static intrinsic explicit operator sbyte(long v);
        public static intrinsic explicit operator sbyte(ulong v);
        public static intrinsic explicit operator sbyte(float v);
        public static intrinsic explicit operator sbyte(double v);
    }
}
