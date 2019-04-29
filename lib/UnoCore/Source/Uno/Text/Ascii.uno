using Uno;

namespace Uno.Text
{
    public static class Ascii
    {
        public static byte[] GetBytes(string value)
        {
            if defined(DOTNET)
                return Encoding.ASCII.GetBytes(value);
            else
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                var res = new byte[value.Length];
                for (var i = 0; i < value.Length; i++)
                    res[i] = (byte)(value[i] < 128 ? value[i] : '?');

                return res;
            }
        }

        public static string GetString(byte[] value)
        {
            if defined(DOTNET)
                return Encoding.ASCII.GetString(value);
            else
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                var res = string.Empty;
                for (var i = 0; i < value.Length; i++)
                    res += value[i] < 128 ? (char)value[i] : '?';

                return res;
            }
        }
    }
}
