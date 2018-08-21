using Uno;
using Uno.Collections;

namespace Uno.Net.Http
{
    class UserInfoParser
    {
        public static string Parse(string uriString, int endIdx, ref int idx)
        {
            if (idx >= endIdx)
            {
                throw new UriFormatException("There is an invalid character sequence in uriString.");
            }

            var end = uriString.IndexOf("@", idx);
            if (end < 0 || end >= endIdx)
            {
                return string.Empty;
            }

            var userInfo = uriString.Substring(idx, end - idx);
            if (!IsValid(userInfo))
            {
                throw new UriFormatException("The user name or password specified in uriString is not valid.");
            }

            idx = end + 1;
            return userInfo;
        }

        private static bool IsValid(string userInfo)
        {
            return userInfo.IndexOf("\\") < 0;
        }
    }
}
