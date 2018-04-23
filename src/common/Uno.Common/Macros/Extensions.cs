using System.Collections.Generic;

namespace Uno.Macros
{
    public static class Extensions
    {
        public static int MacroIndexOf(this string str, char c, int start = 0)
        {
            int cc = 0, pc = 0, ac = 0;
            char q = '\0';

            for (int i = start; i < str.Length; i++)
            {
                if (str[i] == c && cc == 0 && pc == 0 && ac == 0 && q == '\0')
                    return i;

                switch (str[i])
                {
                    case '{':
                        if (q == '\0')
                            cc++;
                        break;
                    case '}':
                        if (q == '\0')
                            cc--;
                        break;
                    case '(':
                        if (q == '\0')
                            pc++;
                        break;
                    case ')':
                        if (q == '\0')
                            pc--;
                        break;
                    case '<':
                        if (cc == 0 && pc == 0 && q == '\0') // only count angle brackets at the outermost level
                        {
                            ac++;
                        }
                        break;
                    case '>':
                        if (cc == 0 && pc == 0 && ac > 0 && q == '\0')
                        {
                            ac--;
                        }
                        break;

                    case '"':
                    case '\'':
                    {
                        // Count backslashes
                        int bsCount = 0;
                        for (int bi = i - 1; bi >= 0 && str[bi] == '\\'; bi--)
                            bsCount++;

                        // Escape backslashes and set quote mode
                        if ((bsCount & 1) == 0)
                        {
                            if (q == str[i])
                                q = '\0';
                            else if (q == '\0')
                                q = str[i];
                        }

                        break;
                    }
                }
            }

            return -1;
        }

        public static List<string> MacroSplit(this string str, char c)
        {
            var result = new List<string>();

            while (true)
            {
                int i = str.MacroIndexOf(c);
                if (i == -1) break;

                result.Add(str.Substring(0, i));
                str = str.Substring(i + 1);
            }

            result.Add(str);
            return result;
        }

        public static int CountNewlines(this string str)
        {
            int r = 0;

            for (int i = 0; i < str.Length; i++)
                if (str[i] == '\n')
                    r++;

            return r;
        }

        public static int CountNewlines(this string str, int start, int end)
        {
            int r = 0;

            for (int i = start; i < end; i++)
                if (str[i] == '\n')
                    r++;

            return r;
        }

        public static Source CreateSource(this string str, Source src, int start, int end)
        {
            // TODO: Probably not correct, check later
            var i = str.LastIndexOf('\n');
            var len = end - start;
            var column = i == -1
                ? src.Column + len
                : len - i;
            return new Source(src.File, src.Line, column, end - start);
        }
    }
}

