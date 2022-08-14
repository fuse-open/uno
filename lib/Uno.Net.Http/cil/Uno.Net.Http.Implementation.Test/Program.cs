using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uno.Net.Http.Implementation.Test
{
    public class HttpRequestCallbacks : IHttpMessageHandlerRequestCallbacks
    {
        public IHttpRequest HttpRequest { get; set; }

        public void OnAborted()
        {
            Console.WriteLine("Request aborted");
        }

        public void OnError(string message)
        {
            Console.WriteLine("ERROR!!!!!!!!!! " + message);
        }

        public void OnTimeout()
        {
            Console.WriteLine("Request timed out");
        }

        public void OnDone()
        {
            Console.WriteLine("Status: " + HttpRequest.GetResponseStatus());
            Console.WriteLine("Headers: " + HttpRequest.GetResponseHeaders());
            var responseContentString = HttpRequest.GetResponseContentString();
            if (responseContentString != null && responseContentString.Length > 15) Console.WriteLine("Body: " + responseContentString.Substring(0,14));
            Console.WriteLine("Body bytes: " + HttpRequest.GetResponseContentByteArray().Length);
        }

        public void OnHeadersReceived()
        {
            Console.WriteLine("Headers received: " + HttpRequest.GetResponseHeaders());
        }

        public void OnProgress(int current, int total, bool hasTotal)
        {
            double i = 100f / total;
            int d = (int)(i * current);

            Console.Write(".");
        }
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Test1();
            Console.ReadKey();
        }

        static IHttpRequest CreateRequest(string method, string url)
        {
            var callbacks = new HttpRequestCallbacks();
            var request = CilHttpRequest.Create(callbacks, method, url);

            callbacks.HttpRequest = request;

            return request;
        }

        static void Update()
        {
            CilHttpMessageHandler.ProcessEvents();
        }

        static void Test1()
        {
            var req = CreateRequest("POST", "http://outracks.com");

            req = CreateRequest("GET", "http://httpbin.org/status/101");

            req.SetHeader("Content-type", "application/json");
            req.SendAsync();

            while (true)
            {
                Update();
                //Console.WriteLine(".");
                Thread.Sleep(100);
            }
        }
    }
}
