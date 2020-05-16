using System.Collections.Generic;
using System.Text;

namespace Uno.Compiler.Core.Syntax.Macros
{
    static class MacroArguments
    {
        static readonly List<string> _empty = new List<string>(0);

        public static List<string> Concat(string obj, List<string> optionalArgs)
        {
            if (optionalArgs == null)
                optionalArgs = _empty;

            var result = new List<string>(1 + optionalArgs.Count) {obj};
            result.AddRange(optionalArgs);
            return result;
        }

        public static bool IsLiteral(string str)
        {
            if (str != null && str.Length >= 2)
            {
                var s = str[0];
                var e = str[str.Length - 1];
                return s == e && (s == '\'' || s == '"');
            }

            return false;
        }

        public static string GetLiteral(string str)
        {
            var sb = new StringBuilder();
            sb.Append('\'');

            foreach (var c in str)
            {
                switch (c)
                {
                    case '\'':
                    case '\\':
                        sb.Append('\\');
                        break;
                }

                sb.Append(c);
            }

            sb.Append('\'');
            return sb.ToString();
        }

        public static string Parse(string str)
        {
            if (IsLiteral(str))
            {
                var sb = new StringBuilder();

                for (int i = 1; i < str.Length - 1; i++)
                {
                    var c = str[i];

                    switch (c)
                    {
                        case '\\':
                            sb.Append(GetEscaped(str[++i]));
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                }

                return sb.ToString();
            }

            return str;
        }

        static string GetEscaped(char c)
        {
            switch (c)
            {
                case '0': return "\0";
                case 'a': return "\a";
                case 'b': return "\b";
                case 'f': return "\f";
                case 'n': return "\n";
                case 'r': return "\r";
                case 't': return "\t";
                case 'v': return "\v";
                case '\'': return "\'";
                case '\"': return "\"";
                case '\\': return "\\";
                default: return "\\" + c;
            }
        }
    }
}
