using Uno;
using Uno.Collections;

namespace Uno.Net.Http
{
    class FragmentParser
    {
        public static string Parse(string uriString, int idx, ref int startPartIdx)
        {
            if (idx >= uriString.Length)
            {
                return string.Empty;
            }

            var start = uriString.IndexOf('#', idx);
            if (start >= 0)
            {
                startPartIdx = start;
                return uriString.Substring(startPartIdx);
            }

            return string.Empty;
        }
    }
}
