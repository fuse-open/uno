using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.Macros
{
    public static class MacroParser
    {
        public static void GetCalls(Source src, string macro, List<MacroCall> result)
        {
            result.Add(new MacroCall());
            int ci = macro.MacroIndexOf(':');

            // '::' should not be interpreted as action, enables use of 'global::'
            while (ci != -1 && ci + 1 < macro.Length && macro[ci + 1] == ':')
                ci = macro.MacroIndexOf(':', ci + 2);

            if (ci == -1)
            {
                var pi = macro.MacroIndexOf('(');

                if (pi == -1 || result.Count == 1)
                {
                    result.Last().Root = macro.Trim();
                    return;
                }

                for (int i = 0; i < pi; i++)
                {
                    if (!char.IsLetter(macro[i]))
                    {
                        result.Last().Root = macro.Trim();
                        return;
                    }
                }
            }
            else
                result.Last().Root = macro.Substring(0, ci).Trim();

            var method = macro.Substring(ci + 1).Trim();
            int li = method.MacroIndexOf('(');
            ci = method.MacroIndexOf(':');

            if (ci != -1 && (li == -1 || ci < li))
            {
                result.Last().Method = method.Substring(0, ci);
                GetCalls(src, method.Substring(ci), result);
                return;
            }

            if (li == -1)
            {
                result.Last().Method = method;
                return;
            }

            int ri = method.MacroIndexOf(')', li + 1);

            if (ri == -1)
                throw new SourceException(src, method.Quote() + " is not a valid macro call");

            result.Last().Method = method.Substring(0, li).Trim();
            var argsPart = method.Substring(li + 1, ri - li - 1).Trim();

            result.Last().Arguments = argsPart.Length > 0
                ? argsPart.MacroSplit(',')
                : new List<string>();

            for (int a = 0; a < result.Last().Arguments.Count; a++)
                result.Last().Arguments[a] = result.Last().Arguments[a].Trim();

            if (ri + 1 < method.Length) {
                if (method[ri + 1] == '.')
                    GetCalls(src, method.Substring(ri + 2), result);
                else if (method[ri + 1] == ':')
                    GetCalls(src, method.Substring(ri + 1), result);
                else
                    throw new SourceException(src, method.Quote() + " is not a valid macro call");
            }
        }

        public static string Expand(
            Source src,
            string text,
            bool escape,
            object context,
            Func<Source, string, object, string> getValue,
            string begin,
            char end,
            Stack<string> stack = null)
        {
            if (text == null || !text.Contains(begin))
                return text;

            int start = 0, i = 0;
            var sb = new StringBuilder();
            var beginLength = begin.Length;

            while (i < text.Length)
            {
                int di = text.IndexOf(begin, i, StringComparison.InvariantCulture);

                // Not found
                if (di == -1)
                {
                    sb.Append(text, i, text.Length - i);
                    break;
                }

                // Count backslashes
                int bsCount = 0;
                for (int bi = di - 1; bi >= 0 && text[bi] == '\\'; bi--)
                    bsCount++;

                sb.Append(text, i, di - i - bsCount);
                i = di;

                // Escape backslashes
                sb.Append(new string('\\', bsCount / 2));

                // Escape macro (when bsCount is odd)
                if ((bsCount & 1) == 1)
                {
                    if (escape)
                        sb.Append(new string('\\', bsCount - bsCount / 2));

                    sb.Append(begin);
                    i += beginLength;
                    continue;
                }

                int ri = text.MacroIndexOf(end, di + beginLength);

                // Not found if next char is whitespace, this workaround enables doxygen groups (/** @{ */ ... /** @} */)
                var wsi = di + beginLength;
                if (wsi < text.Length && char.IsWhiteSpace(text[wsi]))
                {
                    sb.Append(begin);
                    i += beginLength;
                    continue;
                }
                // Invalid macro
                if (ri == -1)
                {
                    throw new SourceException(src, text.Substring(di, text.Length - di).Quote() + " is not a valid macro");
                }

                var name = text.Substring(di + beginLength, ri - di - beginLength);

                if (string.IsNullOrEmpty(name))
                    throw new SourceException(src, "Invalid macro without a name");

                if (stack == null)
                    stack = new Stack<string>();
                else if (stack.Contains(name))
                    throw new SourceException(src, (begin + name + end).Quote() + " creates a circular reference");

                stack.Push(name);

                src = text.CreateSource(src, start, di);
                start = i;
                i = ri + 1;

                // Syntax sugar:
                // A || B || C  =>  A:Or(@(B:Or(@(C)))
                var pi = name.MacroIndexOf('|');
                if (pi != -1 && pi + 1 < name.Length && name[pi + 1] == '|')
                {
                    var ps = 0;
                    var parts = new List<string>();
                    do
                    {
                        parts.Add(name.Substring(ps, pi - ps).Trim());
                        ps = pi + 2;
                        pi = name.MacroIndexOf('|', ps);
                    } while (pi != -1 && pi + 1 < name.Length && name[pi + 1] == '|');
                    parts.Add(name.Substring(ps, name.Length - ps).Trim());

                    var nameb = new StringBuilder(parts[0]);
                    for (int j = 1; j < parts.Count; j++)
                    {
                        nameb.Append(":Or(" + begin);
                        nameb.Append(parts[j]);
                    }
                    for (int j = 1; j < parts.Count; j++)
                    {
                        nameb.Append(end);
                        nameb.Append(')');
                    }

                    name = nameb.ToString();
                }

                var value = getValue(src, name, context) ?? "";
                var newlines = value.CountNewlines();

                if (newlines != 0)
                    src = new Source(src.File, src.Line - newlines, src.Column, src.Length); // TODO: Check this later

                value = Expand(src, value, escape, context, getValue, begin, end, stack);

                stack.Pop();
                sb.Append(value);
            }

            return sb.ToString();
        }
    }
}
