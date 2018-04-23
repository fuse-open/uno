using Uno;
using Uno.Collections;
using Uno.Text;

namespace Uno.Net.Http
{
    class SegmentsParser
    {
        public static string[] Parse(string uriString, int idx, int endIdx, ref int startPartIdx)
        {
            return SplitSegments(ParseAbsolutePath(uriString, idx, endIdx, ref startPartIdx));
        }

        static string ParseAbsolutePath(string uriString, int idx, int endIdx, ref int startPartIdx)
        {
            var start = uriString.IndexOf("/", idx);
            if (start < 0 || start >= endIdx)
            {
                startPartIdx = endIdx;
                return string.Empty;
            }

            startPartIdx = start;
            return uriString.Substring(start, endIdx - start);
        }

        private static bool EscapeSegmentSymbol(byte symbol)
        {
            if (symbol >= 128)
                return true;

            char ch = (char)symbol;
            if (char.IsLetterOrDigit(ch))
                return false;

            if (char.IsControl(ch))
                return true;

            switch (ch)
            {
                case ' ':
                case '?':
                case '`':
                    return true;
            }

            return false;
        }

        static string EscapeSegment(string stringToEscape)
        {
            byte[] bytes = Utf8.GetBytes(stringToEscape);

            var count = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                count++;
                if (EscapeSegmentSymbol(bytes[i]))
                    count += 2;
            }

            var result = new char[count];
            var index = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                var symbol = bytes[i];
                if (EscapeSegmentSymbol(symbol))
                {
                    result[index++] = '%';
                    result[index++] = UriHelper.GetHexFromNumber(symbol >> 4 & 15);
                    result[index++] = UriHelper.GetHexFromNumber(symbol & 15);
                }
                else
                {
                    result[index++] = (char)symbol;
                }
            }
            return new string(result);
        }

        public static string[] SplitSegments(string absolutePath)
        {
            var segments = new List<string>();
            int segmentStart = 0;
            for (var i = 0; i < absolutePath.Length; ++i)
            {
                if (absolutePath[i] == '/')
                {
                    segments.Add(EscapeSegment(absolutePath.Substring(segmentStart, i - segmentStart)) + "/");
                    segmentStart = i + 1;
                }
            }

            if (segmentStart < absolutePath.Length)
                segments.Add(EscapeSegment(absolutePath.Substring(segmentStart, absolutePath.Length - segmentStart)));

            return segments.ToArray();
        }
    }
}
