using System;
using System.Net;

namespace MyHttpLib
{
    public class HttpResponse
    {
        readonly HttpWebResponse _response;

        public string ReasonPhrase
        {
            get { return _response.StatusDescription; }
        }

        public int StatusCode
        {
            get { return (int) _response.StatusCode; }
        }

        public HttpResponse(HttpWebResponse response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            _response = response;
        }

        public string GetHeaders()
        {
            return _response.Headers.ToString();
        }

        public string GetHeader(string name)
        {
            var values = _response.Headers.GetValues(name);
            return values == null ? "" : string.Join(",", values);
        }
    }
}