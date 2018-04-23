using System;
using System.Collections.Generic;
using System.Linq;
using Minimatch;

namespace Uno.ProjectFormat
{
    class GlobList : List<Tuple<Minimatcher, Source, IncludeItemType, string>>
    {
        public void Add(string dir, string pattern, Source src = null, IncludeItemType type = IncludeItemType.Glob, string cond = null)
        {
            // Skip empty patterns and patterns starting with '#' (comments in .unoignore)
            if (string.IsNullOrWhiteSpace(pattern) || pattern.StartsWith("#"))
                return;

            if (pattern.Contains(".."))
                throw new FormatException("'..' is not valid in glob pattern");

            var options = new Options
            {
                IgnoreCase = true,
                MatchBase = true,
                NoNegate = true,
            };

            Insert(
                Tuple.Create(
                    new Minimatcher(
                        !pattern.Contains('/') || pattern.StartsWith("**")
                            ? pattern
                            : dir.NativeToUnix() + "/" + pattern.TrimStart('.', '/'),
                        options),
                    src,
                    type,
                    cond));
        }
        
        // Insert typed files first for higher precedence
        void Insert(Tuple<Minimatcher, Source, IncludeItemType, string> item)
        {
            if (item.Item3 != 0)
            {
                var i = Count;
                while (i > 0 && this[i - 1].Item3 == 0)
                    i--;

                Insert(i, item);
            }
            else
                Add(item);
        }
    }
}
