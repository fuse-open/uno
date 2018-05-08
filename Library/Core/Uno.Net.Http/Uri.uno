using Uno;
using Uno.Collections;
using Uno.Text;

namespace Uno.Net.Http
{
    public class Uri
    {
        public string AbsolutePath { get { return String.Join(string.Empty, Segments); } }

        public string AbsoluteUri
        {
            get
            {
                var schemePrefix = string.Format("{0}:{1}", Scheme, _hasDoubleSlash ? "//" : string.Empty);

                if (!string.IsNullOrEmpty(UserInfo))
                    schemePrefix = string.Format("{0}{1}@", schemePrefix, UserInfo);

                return string.Format("{0}{1}{2}{3}", schemePrefix,
                    Authority ?? string.Empty,
                    PathAndQuery,
                    Fragment ?? string.Empty);
            }
        }

        public string Authority { get; private set; }

        public string Fragment { get; private set; }

        public string Host { get; private set; }

        public string OriginalString { get; private set; }

        public string PathAndQuery { get { return AbsolutePath + Query; } }

        public int Port { get; private set; }

        public string Query { get; private set; }

        [Obsolete("Use Uri.Fragment instead")]
        public string Hash { get { return Fragment; } }

        public string Scheme { get; private set; }

        public string[] Segments { get; private set; }

        public string UserInfo { get; private set; }

        public Uri(string uriString)
        {
            if (uriString == null)
                throw new ArgumentNullException(nameof(uriString));

            if (uriString == string.Empty)
                throw new UriFormatException("The URI is empty");

            if (uriString.Length > 65519)
                throw new UriFormatException("The length of uriString exceeds 65519 characters.");

            CreateThis(uriString);
        }

        [Obsolete("Use `Query.Substring(1).Split('&')` instead")]
        public Dictionary<string, string> GetQueryParameters()
        {
            var result = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Query))
            {
                var parameters = Query.Substring(1);
                var parts = parameters.Split('&');
                foreach (var part in parts)
                {
                    var index = part.IndexOf("=");
                    if (index != -1)
                        result.Add(part.Substring(0, index), part.Substring(index + 1));
                    else
                        result.Add(part, string.Empty);
                }
            }
            return result;
        }

        bool _hasDoubleSlash;

        private void CreateThis(string uriString)
        {
            var idx = 0;

            OriginalString = uriString;

            uriString = uriString.Trim();

            var schemeResult = SchemeParser.Parse(uriString, ref idx, ref _hasDoubleSlash);
            Scheme = schemeResult.Scheme;

            //Parse Query, Hash and AbsolutePath first
            var queryOrHashPartIdx = uriString.Length;
            var partStartIdx = uriString.Length;
            var absolutePathPartIdx = 0;

            if (schemeResult.Type == UriSchemeType.Unknown)
            {
                // HACK: at least make sure we convey the path for custom URIs.
                Segments = new string[] { uriString.Substring(idx, uriString.Length - idx) };

                return;
            }

            Query = QueryParser.Parse(uriString, Scheme, idx, ref partStartIdx);
            queryOrHashPartIdx = partStartIdx < queryOrHashPartIdx ? partStartIdx : queryOrHashPartIdx;
            Fragment = FragmentParser.Parse(uriString, idx, ref partStartIdx);
            queryOrHashPartIdx = partStartIdx < queryOrHashPartIdx ? partStartIdx : queryOrHashPartIdx;

            Segments = SegmentsParser.Parse(uriString, idx, queryOrHashPartIdx, ref absolutePathPartIdx);
            if (schemeResult.Type == UriSchemeType.Internet && Segments.Length == 0)
                Segments = new string[] { "/" };

            if (absolutePathPartIdx > idx)
            {
                // This URI got an authority part, parse it here
                UserInfo = UserInfoParser.Parse(uriString, absolutePathPartIdx, ref idx);

                var hostInfo = HostInfoParser.Parse(uriString, Scheme, absolutePathPartIdx, ref idx);
                Host = hostInfo.Host;
                Port = hostInfo.Port;
                Authority = hostInfo.Authority;
            }
            else
            {
                if (Scheme != "file")
                {
                    throw new UriFormatException("Hostname part required in uriString for scheme " + Scheme);
                }
            }
        }

        [Obsolete("Use `String.Format(\"{0}/{1}\", baseUri.TrimEnd(new char[] { '/' }, uri.TrimStart(new char[] { '/' }))` instead")]
        public static string Combine(string baseUri, string uri)
        {
            return String.Format("{0}/{1}",
                baseUri.TrimEnd(new char[] { '/' }),
                uri.TrimStart(new char[] { '/' }));
        }

        public static string EscapeDataString(string stringToEscape)
        {
            byte[] bytes = Utf8.GetBytes(stringToEscape);

            var count = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                count++;
                if (UriHelper.EscapeDataSymbol(bytes[i]))
                    count += 2;
            }

            var result = new char[count];
            var index = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                var symbol = bytes[i];
                if (UriHelper.EscapeDataSymbol(symbol))
                {
                    result[index++] = '%';
                    result[index++] = UriHelper.GetHexFromNumber(symbol>>4 & 15);
                    result[index++] = UriHelper.GetHexFromNumber(symbol & 15);
                }
                else
                {
                    result[index++] = (char)symbol;
                }
            }
            return new string(result);
        }

        [Obsolete("Use `Uri.EscapeDataString(string)` instead")]
        public static string Encode(string value)
        {
            return EscapeDataString(value);
        }


        public static string UnescapeDataString(string stringToUnescape)
        {
            var count = 0;
            var buffer = new byte[stringToUnescape.Length];
            for (var i = 0; i < stringToUnescape.Length; i++)
            {
                var symbol = stringToUnescape[i];
                if (symbol == '%' && i + 2 < stringToUnescape.Length)
                {
                    var first = UriHelper.GetNumberFromHex(stringToUnescape[i+1]);
                    var second = UriHelper.GetNumberFromHex(stringToUnescape[i+2]);
                    if (first >= 0 && second >= 0)
                    {
                        buffer[count++] = (byte)(first<<4 | second);
                        i += 2;
                    }
                } else
                    buffer[count++] = (byte)symbol;
            }
            if (count < stringToUnescape.Length)
            {
                var result = new byte[count];
                Array.Copy(buffer, result, count);
                return Utf8.GetString(result);
            }
            return Utf8.GetString(buffer);
        }

        [Obsolete("Use `Uri.UnescapeDataString(string)` instead")]
        public static string Decode(string value)
        {
            return UnescapeDataString(value);
        }
    }
}
