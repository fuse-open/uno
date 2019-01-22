using System.Text;

namespace Uno.Configuration.Format
{
    public static class Strings
    {
        public static string Literal(this object o)
        {
            if (o == null)
                return "null";

            if (o is bool)
                return (bool) o
                    ? "true"
                    : "false";

            var s = o.ToString();
            if (s != "if" &&
                s != "else" &&
                s != "null" &&
                s != "include" &&
                s != "require" &&
                IsKey(s))
                return s;

            var sb = new StringBuilder();
            sb.Append('"');

            foreach (var c in s)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\'':
                        sb.Append("\\\'");
                        break;
                    case '\0':
                        sb.Append("\\0");
                        break;
                    case '\a':
                        sb.Append("\\a");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\v':
                        sb.Append("\\v");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            sb.Append('"');
            return sb.ToString();
        }

        public static bool IsIdentifier(this string s)
        {
            if (string.IsNullOrEmpty(s) || char.IsNumber(s[0]))
                return false;

            foreach (var c in s)
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;

            return true;
        }

        public static char Unescape(this char c)
        {
            switch (c)
            {
                case '0':
                    return '\0';
                case 'a':
                    return '\a';
                case 'b':
                    return '\b';
                case 'f':
                    return '\f';
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                case 'v':
                    return '\v';
                default:
                    return c;
            }
        }

        public static bool IsKey(this string s)
        {
            foreach (var c in s)
                if (!c.IsKey())
                    return false;

            return s.Length > 0;
        }

        public static bool IsKey(this char c, bool isConditional = false)
        {
            switch (c)
            {
                case '_':
                    return true;
                case '~':
                case '-':
                case '.':
                case '*':
                case '/':
                case '@':
                case '$':
                case '%':
                case '(':
                case ')':
                    return !isConditional;
                default:
                    return char.IsLetterOrDigit(c);
            }
        }
    }
}