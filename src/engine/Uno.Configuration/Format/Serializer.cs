using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Uno.Configuration.Format
{
    public static class Serializer
    {
        public const string DefaultIndent = "  ";

        public static string StringifyStuff<T>(
            this IDictionary<string, T> stuff,
            bool optimize = false,
            string indent = DefaultIndent)
        {
            var sb = new StringBuilder();

            foreach (var item in stuff)
            {
                object obj = item.Value;
                var str = obj?.ToString();
                if (str != null && str.IndexOf('\n') != -1)
                    obj = str.Split('\n');

                if (!(obj is string) && obj is IEnumerable)
                {
                    var list = item.Value as IReadOnlyList<object>;
                    if (list == null)
                    {
                        list = new List<object>();
                        foreach (var e in (IEnumerable) obj)
                            ((List<object>) list).Add(e);
                    }

                    if (optimize)
                    {
                        switch (list.Count)
                        {
                            case 0:
                                continue;
                            case 1:
                                sb.Append(item.Key.Literal());
                                sb.Append(": ");
                                sb.AppendLine(list[0].Literal());
                                continue;
                        }
                    }

                    sb.Append(item.Key.Literal());
                    sb.Append(": [\n");

                    foreach (var e in list)
                    {
                        sb.Append(indent);
                        sb.AppendLine(e.Literal());
                    }

                    sb.Append(']');
                }
                else
                {
                    if (optimize && (
                            string.IsNullOrEmpty(str) ||
                            obj == null || obj.Equals(false) || obj.Equals(0)))
                        continue;

                    sb.Append(item.Key.Literal());
                    sb.Append(": ");
                    sb.Append(str.Literal());
                }

                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }

        public static void SaveStuff<T>(
            this IDictionary<string, T> stuff,
            string filename,
            bool optimize = false,
            string indent = DefaultIndent)
        {
            File.WriteAllText(filename, stuff.StringifyStuff(optimize, indent));
        }

        public static void WriteConditional(this TextWriter w, string condition, string stuff,
            string indent = DefaultIndent)
        {
            if (!string.IsNullOrEmpty(condition))
            {
                w.WriteLine("if " + condition + " {");
                w.WriteLine(indent + stuff.Replace("\n", "\n" + indent));
                w.WriteLine("}");
            }
            else
                w.WriteLine(stuff);
        }
    }
}