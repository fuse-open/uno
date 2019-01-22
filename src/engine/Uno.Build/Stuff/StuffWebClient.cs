using System;
using System.Net;
using Uno.Diagnostics;

namespace Uno.Build.Stuff
{
    class StuffWebClient : WebClient
    {
        static readonly string UserAgent = "uno/" + UnoVersion.InformationalVersion;

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest) WebRequest.Create(address);
            request.AllowAutoRedirect = true;
            request.UserAgent = UserAgent;
            return request;
        }
    }
}
