using Uno;
using Uno.Collections;

namespace Uno.Net.Http
{
    struct SchemeParserResult
    {
        public UriSchemeType Type;
        public string Scheme;
    }

    class SchemeParser
    {

        public static SchemeParserResult Parse(string uriString, ref int idx, ref bool hasDoubleSlash)
        {
            SchemeParserResult result;

            int end = uriString.IndexOf(":", idx);
            if (end < 0)
            {
                throw new UriFormatException("The scheme isn't specified in uriString.");
            }

            //Scheme must have at least 3 characters and at least 1 before ':'
            if (idx + 2 >= uriString.Length || end == idx)
            {
                throw new UriFormatException("The scheme specified in uriString is not correctly formed.");
            }

            //The length of the scheme specified in uriString should be less than 1023 symbols
            if (end - idx > 1023)
            {
                throw new UriFormatException("The length of the scheme specified in uriString exceeds 1023 characters.");
            }

            result.Scheme = uriString.Substring(idx, end - idx).ToLower();
            result.Type = UriScheme.GetSchemeType(result.Scheme);

            idx = end + 1;
            if (result.Type == UriSchemeType.Unknown)
            {
                return result;//throw new UriFormatException("Unknown scheme specified in uriString.");
            }

            hasDoubleSlash = false;

            // next 2 characters are "//"
            if (idx + 1 < uriString.Length && uriString[idx] == '/' && uriString[idx + 1] == '/')
            {
                hasDoubleSlash = true;
                idx += 2;
            }

            if (result.Type == UriSchemeType.Internet && !hasDoubleSlash)
                throw new UriFormatException("There is an invalid character sequence in uriString.");

            return result;
        }
    }
}
