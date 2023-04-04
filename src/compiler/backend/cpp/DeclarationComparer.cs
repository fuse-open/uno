using System.Collections.Generic;

namespace Uno.Compiler.Backends.CPlusPlus
{
    /** A comparer that makes strings starting with '#' come out first. */
    public class DeclarationComparer : IComparer<string>
    {
        public static readonly DeclarationComparer Singleton = new DeclarationComparer();

        public int Compare(string x, string y)
        {
            var xIsDirective = x.Length > 0 && x[0] == '#';
            var yIsDirective = y.Length > 0 && y[0] == '#';

            return xIsDirective && !yIsDirective
                ? -1
                : !xIsDirective && yIsDirective
                ? 1
                : x.CompareTo(y);
        }
    }
}
