using Uno.Collections;

namespace Uno
{
    //https://msdn.microsoft.com/en-us/library/system.char.ispunctuation(v=vs.110).aspx 2015-03-13
    class CharPunctuationChecker
    {
        class PunctuationRange
        {
            internal char Start { get; set; }
            internal char End { get; set; }

            internal PunctuationRange(char start, char end)
            {
                Start = start;
                End = end;
            }
        }

        private static string singlePunctuationChars = "\u003A\u003B\u003F\u0040\u005F\u007B\u007D\u00A1\u00AB\u00AD\u00B7\u00BB\u00BF\u037E\u0387\u0589\u058A\u05BE\u05C0\u05C3\u05C6" +
                                                       "\u05F3\u05F4\u060C\u060D\u061B\u061E\u061F\u06D4\u0964\u0965\u0970\u0DF4\u0F85\u0FD0\u0FD1\u10FB\u166D\u166E\u169B\u169C\u1735" +
                                                       "\u1736\u1944\u1945\u19DE\u19DF\u1A1E\u1A1F\u207D\u207E\u208D\u208E\u2329\u232A\u29FC\u29FD\u2CFE\u2CFF\u2E1C\u2E1D\u3030\u303D" +
                                                       "\u30A0\u30FB\uFD3E\uFD3F\uFE63\uFE68\uFE6A\uFE6B\uFF1A\uFF1B\uFF1F\uFF20\uFF3F\uFF5B\uFF5D";

        private static List<PunctuationRange> punctuationRangeList = new List<PunctuationRange>()
        {
            new PunctuationRange('\u0021', '\u0023'),
            new PunctuationRange('\u0025', '\u002A'),
            new PunctuationRange('\u002C', '\u002F'),
            new PunctuationRange('\u005B', '\u005D'),
            new PunctuationRange('\u055A', '\u055F'),
            new PunctuationRange('\u066A', '\u066D'),
            new PunctuationRange('\u0700', '\u070D'),
            new PunctuationRange('\u07F7', '\u07F9'),
            new PunctuationRange('\u0E4F', '\u0E5B'),
            new PunctuationRange('\u0F04', '\u0F12'),
            new PunctuationRange('\u0F3A', '\u0F3D'),
            new PunctuationRange('\u104A', '\u104F'),
            new PunctuationRange('\u1361', '\u1368'),
            new PunctuationRange('\u16EB', '\u16ED'),
            new PunctuationRange('\u17D4', '\u17D6'),
            new PunctuationRange('\u17D8', '\u17DA'),
            new PunctuationRange('\u1800', '\u180A'),
            new PunctuationRange('\u1B5A', '\u1B60'),
            new PunctuationRange('\u2010', '\u2027'),
            new PunctuationRange('\u2030', '\u2043'),
            new PunctuationRange('\u2045', '\u2051'),
            new PunctuationRange('\u2053', '\u205E'),
            new PunctuationRange('\u2768', '\u2775'),
            new PunctuationRange('\u275C', '\u27C6'),
            new PunctuationRange('\u27E6', '\u27EB'),
            new PunctuationRange('\u2983', '\u2998'),
            new PunctuationRange('\u29D8', '\u29DB'),
            new PunctuationRange('\u2CF9', '\u2CFC'),
            new PunctuationRange('\u2E00', '\u2E17'),
            new PunctuationRange('\u3001', '\u3003'),
            new PunctuationRange('\u3008', '\u3011'),
            new PunctuationRange('\u3014', '\u301F'),
            new PunctuationRange('\uA874', '\uA877'),
            new PunctuationRange('\uFE10', '\uFE19'),
            new PunctuationRange('\uFE30', '\uFE52'),
            new PunctuationRange('\uFE54', '\uFE61'),
            new PunctuationRange('\uFF01', '\uFF03'),
            new PunctuationRange('\uFF05', '\uFF0A'),
            new PunctuationRange('\uFF0C', '\uFF0F'),
            new PunctuationRange('\uFF3B', '\uFF3D'),
            new PunctuationRange('\uFF5F', '\uFF65')
        };

        internal static bool CheckPunctuation(char c)
        {
            if  (singlePunctuationChars.IndexOf(c) != -1)
                return true;

            foreach (var punctuationRange in punctuationRangeList)
            {
                if (c >= punctuationRange.Start && c <= punctuationRange.End)
                    return true;
            }

            return false;
        }
    }
}
