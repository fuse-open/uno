using System;
using System.Collections.Generic;

namespace Uno.Compiler.Backends.CPlusPlus
{
    /** A comparer that makes strings starting with '#' come out first. */
    public class DeclarationComparer : IComparer<string>
    {
        public static readonly DeclarationComparer Singleton = new();

        public int Compare(string x, string y)
        {
            var xIsDirective = FirstNonWhiteSpaceChar(x) == '#';
            var yIsDirective = FirstNonWhiteSpaceChar(y) == '#';

            return xIsDirective && !yIsDirective
                ? -1
                : !xIsDirective && yIsDirective
                ? 1
                : StringComparer.InvariantCulture.Compare(x, y);
        }

        static char FirstNonWhiteSpaceChar(string value)
        {
            for (int i = 0; i < value.Length; i++)
                if (!char.IsWhiteSpace(value[i]))
                    return value[i];

            return '\0';
        }
    }
}
