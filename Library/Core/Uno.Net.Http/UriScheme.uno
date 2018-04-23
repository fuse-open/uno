using Uno;
using Uno.Collections;

namespace Uno.Net.Http
{
    enum UriSchemeType
    {
        Internet,
        NonInternet,
        Unknown
    }

    class UriScheme
    {
        private static Dictionary<string, int> _defaultPorts = null;

        public static List<string> KnownInternetSchemes = new List<string>() { "http", "https", "ftp", "gopher", "nntp", "telnet", "wais", "file", "prospero", "ws", "wss" };

        public static List<string> KnownNonInternetSchemes = new List<string>() { "mailto", "news" };

        public static Dictionary<string, int> DefaultPorts
        {
            get
            {
                if (_defaultPorts == null)
                {
                    _defaultPorts = new Dictionary<string, int>();
                    _defaultPorts.Add("http", 80);
                    _defaultPorts.Add("https", 443);
                    _defaultPorts.Add("ftp", 21);
                    _defaultPorts.Add("gopher", 70);
                    _defaultPorts.Add("telnet", 119);
                    _defaultPorts.Add("wais", -1);
                    _defaultPorts.Add("file", -1);
                    _defaultPorts.Add("prospero", -1);
                    _defaultPorts.Add("mailto", 25);
                    _defaultPorts.Add("news", -1);
                    _defaultPorts.Add("ws", 80);
                    _defaultPorts.Add("wss", 443);
                }

                return _defaultPorts;
            }
        }

        public static UriSchemeType GetSchemeType(string scheme)
        {
            var s = scheme.ToLower();
            if (KnownInternetSchemes.Contains(s))
            {
                return UriSchemeType.Internet;
            }
            if (KnownNonInternetSchemes.Contains(s))
            {
                return UriSchemeType.NonInternet;
            }
            return UriSchemeType.Unknown;
        }

        public static bool IsHttpScheme(string scheme)
        {
            var s = scheme.ToLower();
            return s == "http" || s == "https";
        }
    }
}
