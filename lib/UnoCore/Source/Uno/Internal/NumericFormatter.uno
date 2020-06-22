using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Text;
using Uno.Math;

namespace Uno.Internal
{
    internal enum FormatSpecifier
    {
        Decimal,
        FixedPoint,
        Hexadecimal,
        Exponential,
        General,
        Percent,
        Number,
        Custom
    }

    public static class NumericFormatter
    {
        private static readonly int NumberDecimalDigits = 2;
        private static readonly int ExponentialDigits = 6;

        static string _decimalPoint;
        private static string DecimalPoint
        {
            get
            {
                if (_decimalPoint == null)
                    _decimalPoint = 1.1.ToString()[1].ToString();
                return _decimalPoint;
            }
        }

        // Public methods

        public static string Format(string formatString, bool i)
        {
            return i.ToString();
        }

        public static string Format(string formatString, sbyte b)
        {
            if (GetFormatSpecifier(formatString) == FormatSpecifier.Hexadecimal)
                return FormatHex(formatString, (ulong)(byte)b, 8);
            return Format(formatString, (long)b);
        }

        public static string Format(string formatString, short s)
        {
            if (GetFormatSpecifier(formatString) == FormatSpecifier.Hexadecimal)
                return FormatHex(formatString, (ulong)(ushort)s, 8);
            return Format(formatString, (long)s);
        }

        public static string Format(string formatString, int i)
        {
            if (GetFormatSpecifier(formatString) == FormatSpecifier.Hexadecimal)
                return FormatHex(formatString, (ulong)(uint)i, 8);
            return Format(formatString, (long)i);
        }

        public static string Format(string formatString, long i)
        {
            if (GetFormatSpecifier(formatString) == FormatSpecifier.Hexadecimal)
                return FormatHex(formatString, (ulong)i, 16);
            else if (i >= 0)
                return Format(formatString, (ulong)i);
            else
                return "-" + Format(formatString, (ulong)(i*-1));
        }

        public static string Format(string formatString, ulong i)
        {
            switch (GetFormatSpecifier(formatString))
            {
                case FormatSpecifier.Decimal:
                    return FormatDecimal(formatString, i);
                case FormatSpecifier.FixedPoint:
                    return FormatFixedPoint(formatString, i);
                case FormatSpecifier.Hexadecimal:
                    return FormatHex(formatString, i, 16);
                case FormatSpecifier.Exponential:
                    return FormatExponential(formatString, (double)i);
                case FormatSpecifier.General:
                    return FormatGeneral(formatString, i);
                case FormatSpecifier.Percent:
                    return FormatNumber(formatString, 100 * i) + " %";
                case FormatSpecifier.Number:
                    return FormatNumber(formatString, i);
                default:
                    return FormatCustom(formatString, i);
            }
        }

        public static string Format(string formatString, float f)
        {
            if (GetFormatSpecifier(formatString) == FormatSpecifier.General)
                return FormatGeneral(formatString, f);
            return Format(formatString, (double)f);
        }

        public static string Format(string formatString, double d)
        {
            switch (GetFormatSpecifier(formatString))
            {
                case FormatSpecifier.FixedPoint:
                    return FormatFixedPoint(formatString, d);
                case FormatSpecifier.Exponential:
                    return FormatExponential(formatString, d);
                case FormatSpecifier.General:
                    return FormatGeneral(formatString, d, 15);
                case FormatSpecifier.Percent:
                    return FormatNumber(formatString, 100 * d) + " %";
                case FormatSpecifier.Number:
                    return FormatNumber(formatString, d);
                case FormatSpecifier.Custom:
                    return FormatCustom(formatString, d);
                default:
                    throw new FormatException("Format specifier was invalid");
            }
        }

        private static string FormatDecimal(string formatString, ulong l)
        {
            var digits = formatString.Length > 1 ? Digits(formatString) : 0;
            return FormatDecimal(l, digits);
        }

        private static string FormatDecimal(ulong l, int digits)
        {
            return l.ToString().PadLeft(digits, '0');
        }

        private static string FormatFixedPoint(string formatString, ulong d)
        {
            return d.ToString() + DecimalPoint + Padding(formatString.Length > 1 ? Digits(formatString) : NumberDecimalDigits);
        }

        // open-coded version of Math.Round(double, int)
        private static double RoundToDigits(double value, int digits)
        {
            long multiplier = (long)Math.Pow(10, digits);
            long intPart = (long)value;
            double decimalPart = Math.Round((value - intPart) * multiplier) / multiplier;
            return intPart + decimalPart;
        }

        private static string FormatFixedPoint(string formatString, double d)
        {
            var desiredDigits = formatString.Length > 1 ? Digits(formatString) : NumberDecimalDigits;
            return FormatFixedPoint(d, desiredDigits);
        }

        [extern(CPLUSPLUS) Require("Source.Include", "stdio.h")]
        [extern(CPLUSPLUS) Require("Source.Include", "errno.h")]
        private static string FormatFixedPoint(double d, int desiredDigits)
        {
            if defined(CIL)
                return System.String.Format("{0:F" + desiredDigits + "}", d);
            else if defined(CPLUSPLUS)
            @{
                // make sure -0 gets formated as 0
                if (d == 0.0)
                    d = 0.0;

                char buf[64];
                int len = snprintf(buf, sizeof(buf), "%.*f", desiredDigits, d);
                if (len < 0 && errno == ERANGE)
                {
                    // Some snprintf implementations return -1 and sets errno to
                    // ERANGE instead of returning the desired length, so let's
                    // reconstruct the value we want here.
                    len = snprintf(nullptr, 0, "%.*f", desiredDigits, d);
                    U_ASSERT(len > sizeof(buf));
                }

                char* ptr = buf;
                if (len > sizeof(buf))
                {
                    // Stackalloc bigger buffer, and try again
                    ptr = (char*)alloca(len + 1);
                    len = snprintf(ptr, len + 1, "%.*f", desiredDigits, d);
                }

                U_ASSERT(len >= 0);
                return uString::Ansi(ptr, len);
            @}
            else
                build_error;
        }

        static readonly char[] lowerHexChars = new char[] {'0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f'};
        static readonly char[] upperHexChars = new char[] {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};

        private static string FormatHex(ulong l, int maxLength, bool upperCase)
        {
            var hexChars = upperCase ? upperHexChars : lowerHexChars;

            var buffer = new char[maxLength];
            int index = maxLength;
            do
            {
                buffer[--index] = hexChars[(int)l & 0xF];
                l >>= 4;
            }
            while (l != 0);

            return new string(buffer, index, maxLength - index);
        }

        private static string FormatHex(string formatString, ulong l, int maxLength)
        {
            var desiredLength = formatString.Length > 1 ? Digits(formatString) : 0;
            return Pad(FormatHex(l, maxLength, char.IsUpper(formatString[0])), desiredLength);
        }

        // Exponential

        private static string FormatExponential(double d, int digits, char exponentSymbol)
        {
            int exponent = 0;
            double significand = 0;
            CalculateExponential(d, out significand, out exponent);

            var sb = new StringBuilder();

            if (d < 0)
                sb.Append('-');

            sb.Append(FormatFixedPoint(significand, digits));
            sb.Append(exponentSymbol);
            sb.Append(exponent < 0 ? '-' : '+');
            sb.Append(FormatDecimal((ulong)Math.Abs(exponent), 3));

            return sb.ToString();
        }

        private static string FormatExponential(string formatString, double d)
        {
            var digits = formatString.Length > 1 ? Digits(formatString) : ExponentialDigits;
            var exponentSymbol = char.IsUpper(formatString[0]) ? 'E' : 'e';
            return FormatExponential(d, digits, exponentSymbol);
        }

        // General

        private static string FormatGeneral(string formatString, ulong i)
        {
            if (formatString.Length == 1)
                return i.ToString();
            var desiredDigits = Digits(formatString);
            var str = i.ToString();
            if (desiredDigits >= str.Length || desiredDigits == 0)
                return str;
            var rounded = RoundToDigits((double)i / Pow(10, str.Length-1), desiredDigits - 1).ToString();
            var exponent = (str.Length - 1).ToString();
            return rounded + "E+" + Padding(2 - exponent.Length) + exponent;
        }

        private static string PruneNeedlessDecimals(string str)
        {
            /* Trim decimal zeroes, as well as decimal point if integer
             *
             * TODO: it's be even better if we could find a way of having
             * FormatFixedPoint not add these in the first place.
             */

            if (str.IndexOf('.') >= 0)
                return str.TrimEnd('0').TrimEnd('.');

            return str;
        }

        private static string FormatGeneral(string formatString, float f)
        {
            return FormatGeneral(formatString, (double)f, 7);
        }

        private static string FormatGeneral(string formatString, double d, int defaultPrecision)
        {
            if (d == 0)
                return "0";

            var desiredDigits = Digits(formatString);
            if (desiredDigits < 1)
                desiredDigits = defaultPrecision;

            if (d == 0)
                return "0";

            var magnitude = Log10(Math.Abs(d));

            if (magnitude < desiredDigits)
            {
                var intDigits = (int)Ceil(magnitude);
                var str = FormatFixedPoint(d, desiredDigits - intDigits);
                return PruneNeedlessDecimals(str);
            }

            int exponent = 0;
            double significand = 0;
            CalculateExponential(d, out significand, out exponent);

            var sb = new StringBuilder();

            if (d < 0)
                sb.Append('-');

            var significandDigits = Max(0, desiredDigits - 1);
            significand = Round(significand, significandDigits);
            sb.Append(PruneNeedlessDecimals(FormatFixedPoint(significand, significandDigits)));
            sb.Append(char.IsUpper(formatString[0]) ? 'E' : 'e');
            sb.Append(exponent < 0 ? '-' : '+');
            sb.Append(FormatDecimal((ulong)Abs(exponent), 2));

            return sb.ToString();
        }

        // Number

        private static string FormatNumber(string formatString, ulong i)
        {
            var desiredDigits = formatString.Length > 1 ? Digits(formatString) : NumberDecimalDigits;
            if (desiredDigits == 0)
                return FormatNumber(i);
            return FormatNumber(i) + "." + Padding(desiredDigits);
        }

        private static string FormatNumber(string formatString, double d)
        {
            var desiredDigits = formatString.Length > 1 ? Digits(formatString) : NumberDecimalDigits;
            var rounded = RoundToDigits(d, desiredDigits);
            if (desiredDigits == 0)
                return FormatNumber(rounded);
            var str = rounded.ToString();
            var residue = str.IndexOf(DecimalPoint) == -1 ? string.Empty : str.Substring(str.IndexOf(DecimalPoint) + 1);
            return FormatNumber(rounded) + DecimalPoint + residue + Padding(desiredDigits - residue.Length);
        }

        private static string FormatNumber(double d)
        {
            if (d < 0)
                return "-" + FormatNumber((ulong)-d);
            return FormatNumber((ulong)d);
        }

        private static string FormatNumber(ulong i)
        {
            var str = i.ToString();
            if (str.Length <= 3)
                return str;
            var start = str.Length % 3 == 0 ? 3 : str.Length % 3;
            var result = new Uno.Text.StringBuilder();
            result.Append(str.Substring(0, start));
            for (var index = start; index < str.Length; index += 3)
            {
                result.Append(",");
                result.Append(str.Substring(index, 3));
            }
            return result.ToString();
        }

        // Custom

        private static string FormatCustom(string formatString, ulong value)
        {
            int decimalPoint = formatString.IndexOf('.');
            if (decimalPoint == -1)
                return FormatCustomIntegerPart(formatString, value);

            bool hasDecimalPoint;
            var integerPart = FormatCustomIntegerPart(formatString.Substring(0, decimalPoint), value);
            var doublePart = FormatCustomDoublePart(formatString.Substring(decimalPoint + 1), 0, out hasDecimalPoint);
            return integerPart + (hasDecimalPoint ? "." : string.Empty) + doublePart;
        }

        private static string FormatCustom(string formatString, double value)
        {
            var absoluteValue = Abs(value);
            int decimalPoint = formatString.IndexOf('.');
            if (decimalPoint == -1)
                return FormatCustomIntegerPart(formatString, (ulong)absoluteValue);

            bool hasDecimalPoint;
            var integerPart = FormatCustomIntegerPart(formatString.Substring(0, decimalPoint), (ulong)absoluteValue);
            var doublePart = FormatCustomDoublePart(formatString.Substring(decimalPoint + 1), absoluteValue, out hasDecimalPoint);
            return (value < 0 ? "-" : string.Empty) + integerPart + (hasDecimalPoint ? "." : string.Empty) + doublePart;
        }

        private static string FormatCustomDoublePart(string doubleFormat, double d, out bool decimalPoint)
        {
            var precision = 0;
            bool hasZero = false;
            var processedFormat = doubleFormat.ToCharArray();
            for (var i = processedFormat.Length - 1; i >= 0; i--)
            {
                if (processedFormat[i] == '0')
                    hasZero = true;
                if (processedFormat[i] == '0' || processedFormat[i] == '#')
                {
                    processedFormat[i] = hasZero ? '0' : processedFormat[i];
                    precision++;
                }
            }
            var raw = RoundToDigits(d, precision).ToString();
            var index = raw.IndexOf(DecimalPoint) + 1;
            decimalPoint = index != 0 || hasZero;
            if (index == 0) index = raw.Length;
            var formatted = new List<char>(processedFormat.Length);
            for (var i = 0; i < processedFormat.Length; i++)
            {
                switch(processedFormat[i])
                {
                    case '0':
                        formatted.Add(index >= raw.Length ? '0' : raw[index++]);
                        break;
                    case '#':
                        if (index < raw.Length)
                            formatted.Add(raw[index++]);
                        break;
                    case '.':
                        break;
                    default:
                        formatted.Add(processedFormat[i]);
                        break;
                }
            }
            return new String(formatted.ToArray());
        }

        private static string FormatCustomIntegerPart(string integerFormat, ulong value)
        {
            var raw = value.ToString();
            var rawIndex = value == 0 ? -1 : raw.Length - 1;

            var formatted = new char[integerFormat.Length];
            var index = integerFormat.Length - 1;

            var lastNumber = 0;
            for (var i = integerFormat.Length - 1; i >= 0; i--)
            {
                switch(integerFormat[i])
                {
                    case '0':
                        lastNumber = i;
                        formatted[index--] = rawIndex < 0 ? '0' : raw[rawIndex--];
                        break;
                    case '#':
                        lastNumber = i;
                        if (rawIndex >= 0)
                            formatted[index--] = raw[rawIndex--];
                        break;
                    default:
                        formatted[index--] = integerFormat[i];
                        break;
                }
            }
            var formattedString = new String(formatted);
            if (index != -1)
                formattedString = formattedString.Substring(index + 1);
            if (rawIndex >= 0)
                return formattedString.Insert(lastNumber, raw.Substring(0, rawIndex + 1));
            return formattedString;
        }

        // Common

        private static void CalculateExponential(double d, out double significand, out int exponent)
        {
            exponent = 0;
            significand = 0;
            if (d != 0)
            {
                var abs = Math.Abs(d);
                exponent = (int)Math.Floor(Math.Log10(abs));
                if (exponent < -324 + 16)
                {
                    // prevent inf in Math.Pow(10, -exponent)
                    abs *= 1e16;
                    var pow = Math.Pow(10.0, -(exponent + 16));
                    significand = abs * pow;
                }
                else
                    significand = abs * Math.Pow(10.0, -exponent);
            }
        }

        private static int Digits(string formatString)
        {
            int digits;
            if (!int.TryParse(formatString.Substring(1), out digits))
                return -1;

            return digits;
        }

        private static bool IsLetter(char symbol)
        {
            return symbol >= 'A' && symbol <= 'Z';
        }

        private static string Pad(string unmodified, int minLength)
        {
            var actualLength = unmodified.Length;
            if (minLength <= actualLength)
                return unmodified;

            var padding = Padding(minLength - actualLength);
            return padding + unmodified;
        }

        private static string Pad(string unmodified, string formatString)
        {
            var desiredLength = formatString.Length > 1 ? Digits(formatString) : unmodified.Length;
            return Pad(unmodified, desiredLength);
        }

        private static string Padding(int length)
        {
            if (length <= 0)
                return "";
            var padding = new char[length];
            for (int i = 0; i < length; i++)
                padding[i] = '0';
            return new string(padding);
        }

        private static FormatSpecifier GetFormatSpecifier(string formatString)
        {
            var symbol = Char.ToUpper(formatString[0]);
            if (formatString.Length == 1 && IsLetter(symbol))
                return GetStandartFormat(symbol);

            int decimals = Digits(formatString);
            if (decimals < 0 || decimals > 99)
                return FormatSpecifier.Custom;
            if (formatString.Trim('0').Length == 0)
                return FormatSpecifier.Custom;

            return GetStandartFormat(symbol);
        }

        private static FormatSpecifier GetStandartFormat(char symbol)
        {
            switch (symbol)
            {
                case 'X':
                    return FormatSpecifier.Hexadecimal;
                case 'D':
                    return FormatSpecifier.Decimal;
                case 'F':
                    return FormatSpecifier.FixedPoint;
                case 'G':
                    return FormatSpecifier.General;
                case 'N':
                    return FormatSpecifier.Number;
                case 'E':
                    return FormatSpecifier.Exponential;
                case 'P':
                    return FormatSpecifier.Percent;
                default:
                    throw new FormatException("Format specifier was invalid");
            }
        }
    }
}
