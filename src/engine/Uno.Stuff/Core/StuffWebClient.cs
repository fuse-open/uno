using System;
using System.Net;

namespace Stuff.Core
{
    class StuffWebClient : WebClient
    {
        static readonly string UserAgent = "stuff/" + Command.ExeVersion;

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest) WebRequest.Create(address);
            request.AllowAutoRedirect = true;
            request.UserAgent = UserAgent;
            return request;
        }
    }
}
