using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Build.Targets.Browser
{
    public class WebServer
    {
        protected HttpListener _httpListener = new HttpListener();
        public readonly string WebRoot;

        public WebServer(string prefix, string webroot)
        {
            WebRoot = webroot;
            AddPrefix(prefix);
            Start();
        }

        protected void AddPrefix(string prefix)
        {
            _httpListener.Prefixes.Add(prefix);
        }

        protected void Start()
        {
            _httpListener.Start();

            var task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var context = _httpListener.GetContext();
                        try
                        {
                            HandleRequest(context);
                        }
                        finally
                        {
                            context.Response.Close();
                        }
                    }
                    catch (HttpListenerException e)
                    {
                        if (e.ErrorCode == 995)
                            break;

                        throw e;
                    }
                }
            });
        }

        public void Stop()
        {
            if (_httpListener.IsListening)
                _httpListener.Stop();
        }

        string GetFilePath(Uri uri)
        {
            return Path.Combine(WebRoot, Path.Combine(uri.Segments.Skip(1).ToArray()));
        }

        byte[] GetRequestData(string filePath, HttpListenerResponse response)
        {
            if (!Path.GetFullPath(filePath).StartsWith(WebRoot) || !File.Exists(filePath))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return Encoding.UTF8.GetBytes("<html><body><h1>404 - Not found</h1></body></html>");
            }

            using (var fileStream = File.OpenRead(filePath))
            {
                if (fileStream.Length == 0)
                    return new byte[] { };

                var buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);

                response.StatusCode = (int)HttpStatusCode.OK;
                return buffer;
            }
        }

        protected void HandleRequest(HttpListenerContext context)
        {
            using (var outputStream = context.Response.OutputStream)
            {
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");

                var filePath = GetFilePath(context.Request.Url);
                var contentStream = GetRequestData(filePath, context.Response);

                outputStream.Write(contentStream, 0, contentStream.Length);
            }
        }
    }
}
