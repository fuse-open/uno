using System;
using System.Collections.Generic;

namespace Uno.Configuration.Format
{
    public static class Extensions
    {
        public static bool TryGetValue<T>(this IDictionary<string, T> obj, string key, out string value)
        {
            T str;
            value = null;
            if (obj.TryGetValue(key, out str))
            {
                value = str?.ToString();
                return true;
            }
            return false;
        }

        public static bool TryGetValue<T>(this IDictionary<string, T> obj, string key, out bool value)
        {
            string str;
            value = false;
            if (obj.TryGetValue(key, out str))
            {
                value = bool.Parse(str);
                return true;
            }
            return false;
        }

        public static bool TryGetValue<T>(this IDictionary<string, T> obj, string key, out int value)
        {
            string str;
            value = 0;
            if (obj.TryGetValue(key, out str))
            {
                value = int.Parse(str);
                return true;
            }
            return false;
        }

        public static bool TryGetValue<T, TEnum>(this IDictionary<string, T> obj, string key, out TEnum value)
            where TEnum : struct
        {
            string str;
            value = default(TEnum);
            if (obj.TryGetValue(key, out str))
            {
                value = (TEnum)Enum.Parse(typeof(TEnum), str);
                return true;
            }
            return false;
        }

        public static bool TryGetValue<T, TResult>(this IDictionary<string, T> obj, string key, out TResult value, Func<string, TResult> converter)
        {
            string str;
            value = default(TResult);
            if (obj.TryGetValue(key, out str))
            {
                value = converter(str);
                return true;
            }
            return false;
        }

        public static IEnumerable<TResult> GetArray<T, TResult>(this IDictionary<string, T> obj, string name,
            Func<string, TResult> converter)
        {
            foreach (var e in obj.GetArray(name))
                yield return converter(e);
        }

        public static IEnumerable<string> GetArray<T>(this IDictionary<string, T> obj, string name)
        {
            string array;
            return obj.TryGetValue(name, out array)
                ? array.Lines()
                : new string[0];
        }

        public static IEnumerable<string> Lines(this object array)
        {
            return array?.ToString().Lines();
        }

        public static IEnumerable<string> Lines(this string array)
        {
            if (array != null)
                foreach (var line in array.Split('\n'))
                    if (!string.IsNullOrWhiteSpace(line))
                        yield return line;
        }
    }
}