using Uno;
using Uno.Collections;

namespace Uno.Net.Http
{
    class QueryParser
    {
        public static string Parse(string uriString, string scheme, int idx, ref int startPartIdx)
        {
            if (idx >= uriString.Length || !UriScheme.IsHttpScheme(scheme))
            {
                return string.Empty;
            }

            var startQuery = uriString.IndexOf('?', idx);
            var startHash = uriString.IndexOf('#', idx);
            if (startHash < 0)
            {
                startHash = uriString.Length;
            }

            if (startQuery >= 0 && startQuery < startHash)
            {
                startPartIdx = startQuery;
                return uriString.Substring(startPartIdx, startHash - startPartIdx);
            }

            return string.Empty;
        }
    }
}
