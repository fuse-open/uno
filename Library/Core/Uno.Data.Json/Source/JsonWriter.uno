using Uno.Text;

namespace Uno.Data.Json
{
    public static class JsonWriter
    {
        /** Quote a string for use in a JSON document. Ths method adds quotes
            around the result.

            Always prefers the shortest valid encoding.
        */
        public static string QuoteString(string str)
        {
            var sb = new StringBuilder();
            sb.Append("\"");
            EscapeString(str, sb);
            sb.Append("\"");
            return sb.ToString();
        }

        /** Escape a string for use in a JSON document. This method does not
            add quotes around the result.

            Always prefers the shortest valid encoding.
        */
        public static string EscapeString(string str)
        {
            var sb = new StringBuilder();
            EscapeString(str, sb);
            return sb.ToString();
        }

        static void EscapeString(string str, StringBuilder sb)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
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

                    default:
                        if (ch <= 0x1f)
                            sb.Append(string.Format("\\u{0:x4}", (int)ch));
                        else
                            sb.Append(ch);
                        break;
                }
            }
        }
    }
}
