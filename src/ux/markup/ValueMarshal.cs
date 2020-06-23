using System;
using System.Globalization;
using System.Linq;
using Uno.UX.Markup;
using Uno.UX.Markup.Types;

namespace Uno.UX
{
    public static class ValueMarshal
    {
        const NumberStyles FloatNumberStyle = NumberStyles.Float;

        public static CultureInfo CultureInfo { get; } = CultureInfo.InvariantCulture;

        public static object TryParse(string value, Type type, FileSourceInfo src)
        {
            double d = 0;
            if (type == typeof(string)) return value;
            if (type == typeof(bool)) return value.ToLower() != "false";

            // Temp hack until Markup.Model goes away
            // convert #hex -> float,float,float,float
            if (type.Name.ToLower().StartsWith("float") && value.StartsWith("#"))
            {
                var vector = Markup.AtomicValueParser.ParseFloatVector<float>(value, type.GetFields().Count(x => x.IsPublic), src);
                value = vector.Components.Select(x => x.Value.ToString(CultureInfo)).Aggregate((x, y) => x + ", " + y);
            }

            if (type == typeof(float))
                value = value.Trim('f');

            if ((type.IsIntegerType() || type.IsFloatingPointType()) && double.TryParse(value, FloatNumberStyle, CultureInfo, out d))
            {
                return Convert.ChangeType(d, type, CultureInfo.NumberFormat);
            }
            if (type == typeof(Enum) && double.TryParse(value, FloatNumberStyle, CultureInfo, out d))
            {
                return (int)d;
            }
            if (type.IsEnum)
            {
                try
                {
                    return Enum.Parse(type, value);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            if (type.IsValueType)
            {
                var p = value.Split(',');
                var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (p.Length != fields.Length) return null;

                var obj = System.Activator.CreateInstance(type);

                for (int x = 0; x < fields.Length; x++)
                {
                    var v = TryParse(p[x].Trim(), fields[x].FieldType, src);
                    if (v != null) fields[x].SetValue(obj, v);
                }

                return obj;
            }

            return null;
        }

        public static T Parse<T>(object value, FileSourceInfo src)
        {
            return (T) Parse(Encode(value), typeof (T), src);
        }

        public static object Parse(string value, Type type, FileSourceInfo src)
        {
            var res = TryParse(value, type, src);
            if (res == null) throw new Exception("Unable to parse value of type '" + type.FullName + "'");
            return res;
        }

        public static bool IsFloatingPointType(this Type t)
        {
            return t == typeof (float) ||
                   t == typeof (double);
        }

        public static bool IsIntegerType(this Type t)
        {
            return t == typeof (byte) ||
                   t == typeof (sbyte) ||
                   t == typeof (short) ||
                   t == typeof (ushort) ||
                   t == typeof (int) ||
                   t == typeof (uint) ||
                   t == typeof (long) ||
                   t == typeof (ulong);
        }

        public static bool IsVectorType(this Type t)
        {
            return t == typeof (Float2) ||
                   t == typeof (Float3) ||
                   t == typeof (Float4) ||
                   t == typeof (Int2) ||
                   t == typeof (Int3) ||
                   t == typeof (Int4);
        }

        public static string Encode(object value, bool includeSuffixes = false)
        {
            if (value == null) return null;

            var type = value.GetType();

            if (type == typeof(string)) return value as string;
            if (type == typeof(bool)) { return (bool)value ? "true" : "false"; }
            if (type.IsIntegerType() || type.IsFloatingPointType())
                return Convert.ToString(Convert.ChangeType(value, type), CultureInfo) +
                       (type == typeof (float) && includeSuffixes ? "f" : string.Empty);
            if (type.IsValueType)
            {
                var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                string s = "";
                for (int i = 0; i < fields.Length; i++)
                {
                    if (i > 0) s += ", ";
                    s += Encode(fields[i].GetValue(value), includeSuffixes);
                }

                return s;
            }
            else throw new Exception("Can't encode value of type " + type);
        }
    }
}
