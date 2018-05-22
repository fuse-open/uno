namespace Uno.Net.Http
{
    class UriHelper
    {
        public static char GetHexFromNumber(int value)
        {
            if (value > 9)
                return (char)((byte)'A' + value - 10);
            return (char)((byte)'0' + value);
        }

        public static int GetNumberFromHex(char symbol)
        {
            if (symbol >= '0' && symbol <= '9')
            {
                return ((byte)symbol - (byte)'0');
            }
            char letter = char.ToLower(symbol);
            if (letter >= 'a' && letter <= 'z')
                return ((byte)letter - (byte)'a' + 10);
            return -1;
        }

        public static bool EscapeDataSymbol(byte symbol)
        {
            if (symbol >= 128)
                return true;

            if (char.IsLetter((char)symbol) || char.IsDigit((char)symbol))
                return false;
            switch ((char)symbol)
            {
                case '-':
                case '_':
                case '.':
                case '~':
                    return false;
            }
            return true;
        }
    }
}
