using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Uno.Configuration.Format
{
    public class StuffObject : Dictionary<string, object>
    {
        public static StuffObject Load(string filename, StuffFlags flags = 0, IEnumerable<string> optionalDefines = null, Action<string> printLine = null)
        {
            return Parse(filename, File.ReadAllText(filename), File.ReadAllText, flags, optionalDefines, printLine);
        }

        public static StuffObject Parse(string filename, string stuff, Func<string, string> requireResolver = null, StuffFlags flags = 0, IEnumerable<string> optionalDefines = null, Action<string> printLine = null)
        {
            var file = new StuffFile(
                filename,
                flags.HasFlag(StuffFlags.AcceptAll)
                    ? null
                    : new HashSet<string>(
                        (optionalDefines ?? StuffFile.DefaultDefines)
                            .Select(x => x.ToUpperInvariant())));

            file.Parse(stuff, printLine);
            var result = file.Flatten(requireResolver).ToObject();

            // Workaround to avoid side-effect when StuffFlags.AcceptAll is set
            if (flags.HasFlag(StuffFlags.AcceptAll))
                foreach (var e in Parse(filename, stuff, requireResolver, 0, optionalDefines, printLine))
                    if (!result.ContainsKey(e.Key) || result[e.Key]?.ToString().IndexOf('\n') == -1)
                        result[e.Key] = e.Value;

            printLine?.Invoke(result.StringifyStuff());
            return result;
        }

        internal StuffObject(IDictionary<string, StuffItem> copy)
            : base(copy.Count)
        {
            foreach (var e in copy)
                Add(e.Key, e.Value);
        }

        public StuffObject()
        {
        }

        public void Save(string filename, bool skipEmpty = false, string indent = Serializer.DefaultIndent)
        {
            this.SaveStuff(filename, skipEmpty, indent);
        }
    }
}