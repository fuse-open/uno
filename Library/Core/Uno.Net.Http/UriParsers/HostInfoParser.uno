using Uno;
using Uno.Collections;

namespace Uno.Net.Http
{
    class HostInfo
    {
        public string Authority { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }

    class HostInfoParser
    {
        private static List<string> _invalidChars = new List<string>() { "..", ",", "\\", "|", "<", ">", "'", ";", ":", "[", "]", "{", "}", "(", ")", "=", "+" };

        public static HostInfo Parse(string uriString, string scheme, int endIdx, ref int idx)
        {
            if (idx >= endIdx)
            {
                throw new UriFormatException("There is an invalid character sequence in uriString.");
            }

            var end = uriString.IndexOf("/", idx);
            if (end < 0 || end >= endIdx)
            {
                end = endIdx;
            }

            var hostInfoString = uriString.Substring(idx, end - idx);
            idx = end;

            if (string.IsNullOrEmpty(hostInfoString))
            {
                throw new UriFormatException("The host specified in uriString is not valid or cannot be parsed.");
            }

            var delim = hostInfoString.IndexOf(":");
            var hostInfo = new HostInfo() { Authority = hostInfoString };

            if (delim > 0)
            {
                //port specified
                var portString = hostInfoString.Substring(delim + 1);
                hostInfo.Host = hostInfoString.Substring(0, delim);
                hostInfo.Port = ExtractPort(portString, GetDafaultPort(scheme));
            }
            else
            {
                hostInfo.Host = hostInfoString;
                hostInfo.Port = GetDafaultPort(scheme);
            }

            if (!IsHostValid(hostInfo.Host))
            {
                throw new UriFormatException("The host specified in uriString is not valid or cannot be parsed.");
            }

            return hostInfo;
        }

        private static bool IsHostValid(string userInfo)
        {
            foreach (var chars in _invalidChars)
            {
                if (userInfo.IndexOf(chars) >= 0)
                    return false;
            }

            return !userInfo.StartsWith(".");
        }

        private static int ExtractPort(string portString, int defaultPort)
        {
            var port = 0;
            if (string.IsNullOrEmpty(portString))
            {
                port = defaultPort;
            }
            else
            {
                if (!int.TryParse(portString, out port))
                {
                    throw new UriFormatException("The port number specified in uriString is not valid or cannot be parsed.");
                }
            }

            return port;
        }

        private static int GetDafaultPort(string scheme)
        {
            return UriScheme.DefaultPorts[scheme.ToLower()];
        }
    }
}
