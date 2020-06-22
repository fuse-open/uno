using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Uno.UX.Markup.Types.UX;

namespace Uno.UX.Markup
{
    public static class AtomicValueParser
    {
        public static Size ParseSize(string s, FileSourceInfo src)
        {
            s = s.Trim('f');

            Unit unit = Unit.Unspecified;
            if (s.EndsWith("%"))
            {
                unit = Unit.Percent;
                s = s.Trim('%');
            }
            else if (s.EndsWith("px"))
            {
                unit = Unit.Pixels;
                s = s.Substring(0, s.Length - 2);
            }
            else if (s.EndsWith("pt"))
            {
                unit = Unit.Points;
                s = s.Substring(0, s.Length - 2);
            }

            var d = double.Parse(s, CultureInfo.InvariantCulture);

            return new Size((float)(dynamic)d, unit.ToString(), src);
        }

        public static Size2 ParseSize2(string s, FileSourceInfo src)
        {
            var p = s.Split(',');
            var data = p.Select(x => ParseSize(x, src)).ToArray();

            if (data.Length != 2)
            {
                data = AutoExpandVector(data, 2);

                if (data.Length != 2)
                    throw new Exception("Unexpected component count in Size vector, expected " + 2 + " found " + p.Length);
            }

            return new Size2(data[0], data[1], src);
        }

        public static Scalar<T> ParseFloat<T>(string s, FileSourceInfo src) where T : IFormattable
        {
            s = s.Trim('f');
            var d = double.Parse(s, CultureInfo.InvariantCulture);

            return new Scalar<T>((T)(dynamic)d, src);
        }

        public static Vector<T> ParseFloatVector<T>(string s, int comps, FileSourceInfo src) where T : IFormattable
        {
            s = s.Trim();
            if (s.StartsWith("#")) return ParseHexVector<T>(s, comps, src);

            var p = s.Split(',');
            var data = p.Select(x => ParseFloat<T>(x, src)).ToArray();

            if (data.Length != comps)
            {
                data = AutoExpandVector(data, comps);

                if (data.Length != comps)
                    throw new Exception("Unexpected component count in float vector, expected " + comps + " found " + p.Length);
            }

            return new Vector<T>(data, src);
        }

        public static Vector<T> ParseHexVector<T>(string s, int comps, FileSourceInfo src) where T : IFormattable
        {
            s = s.Trim('#');

            if (comps == 3)
            {
                switch (s.Length) {
                    case 3:
                        return new Vector<T>(s.Select(x => ParseHex<T>(new string(x, 1), src)).ToArray(), src);
                    case 6:
                        var parts = new List<string>();
                        for (int i = 0; i < s.Length; i += 2)
                        {
                            parts.Add(s.Substring(i, 2));

                        }
                        return new Vector<T>(parts.Select(x => ParseHex<T>(x, src)).ToArray(), src);
                    default:
                        throw new Exception("Invalid hex-code for 3-component vector, expected 3 or 6 digits");
                }
            }
            else if (comps == 4)
            {
                switch (s.Length) {
                    case 3:
                        return new Vector<T>((s + "f").Select(x => ParseHex<T>(new string(x, 1), src)).ToArray(), src);
                    case 4:
                        return new Vector<T>(s.Select(x => ParseHex<T>(new string(x, 1), src)).ToArray(), src);
                    case 6:
                    {
                        s += "ff";
                        var parts = new List<string>();
                        for (int i = 0; i < s.Length; i += 2)
                        {
                            parts.Add(s.Substring(i, 2));

                        }
                        return new Vector<T>(parts.Select(x => ParseHex<T>(x, src)).ToArray(), src);
                    }
                    case 8:
                    {
                        var parts = new List<string>();
                        for (int i = 0; i < s.Length; i += 2)
                        {
                            parts.Add(s.Substring(i, 2));

                        }
                        return new Vector<T>(parts.Select(x => ParseHex<T>(x, src)).ToArray(), src);
                    }
                    default:
                        throw new Exception("Invalid hex-code for 4-component vector, expected 3, 4, 6 or 8 digits");
                }
            }
            else
            {
                throw new Exception(comps + "-component vector cannot be encoded as hex value");
            }
        }

        static bool IsIntegralType(Type t)
        {
            return
                t == typeof(uint) || t == typeof(int) ||
                t == typeof(ulong) || t == typeof(long) ||
                t == typeof(ushort) || t == typeof(short) ||
                t == typeof(byte) || t == typeof(sbyte);
        }

        static Scalar<T> ParseHex<T>(string digit, FileSourceInfo src) where T : IFormattable
        {
            uint res;
            if (!uint.TryParse(digit, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res))
                throw new Exception("Unrecognized hex digit: " + digit);

            if (IsIntegralType(typeof(T)))
            {
                return new Scalar<T>((T)(dynamic)res, src);
            }
            else
            {
                double ratio = (double)0xF;
                if (digit.Length == 2) ratio = (double)0xFF;

                return new Scalar<T>((T)(dynamic)((double)res / ratio), src);
            }
        }

        public static Scalar<T> ParseInteger<T>(string s, Func<string, T> parser, FileSourceInfo src) where T : IFormattable
        {
            // TODO: tackle other integer encodings + postfixes

            var value = parser(s);

            return new Scalar<T>(value, src);
        }

        public static Vector<T> ParseIntegerVector<T>(string s, int comps, Func<string, T> parser, FileSourceInfo src) where T : IFormattable
        {
            s = s.Trim();
            if (s.StartsWith("#")) return ParseHexVector<T>(s, comps, src);

            var p = s.Split(',');
            var data = p.Select(x => ParseInteger(x, parser, src)).ToArray();

            if (data.Length != comps)
            {
                data = AutoExpandVector(data, comps);

                if (data.Length != comps)
                    throw new Exception("Unexpected component count in integer vector, expected " + comps + " found " + p.Length);
            }

            return new Vector<T>(data, src);
        }

        static T[] AutoExpandVector<T>(T[] data, int expectedComps)
        {
            var list = new List<T>();

            if (data.Length == 1)
            {
                for (int i = 0; i < expectedComps; i++)
                    list.Add(data[0]);
            }
            else if (data.Length == 2 && expectedComps == 4)
            {
                list.Add(data[0]);
                list.Add(data[1]);
                list.Add(data[0]);
                list.Add(data[1]);
            }
            else
            {
                return data;
            }

            return list.ToArray();
        }
    }
}

