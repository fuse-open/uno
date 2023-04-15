using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Uno.Diagnostics;

namespace Uno.Build.Stuff
{
    class UnoHttpClient : HttpClient
    {
        public UnoHttpClient()
        {
            var userAgent = new ProductInfoHeaderValue("uno", UnoVersion.InformationalVersion);
            DefaultRequestHeaders.UserAgent.Add(userAgent);
        }

        public void DownloadFile(string url, string filename)
        {
            var downloadTask = GetByteArrayAsync(url);
            downloadTask.Wait();
            File.WriteAllBytes(filename, downloadTask.Result);
        }
    }
}
