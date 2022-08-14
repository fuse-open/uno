using Uno;
using Uno.Collections;
using Fuse;
using Uno.Net.Http;

namespace HttpStatusCodeTester
{
    public class HttpWrapper
    {
        HttpMessageHandler _client;
        Action<HttpEvent> _callback;

        public HttpWrapper(Action<HttpEvent> callback)
        {
            _client = new HttpMessageHandler();
            _callback = callback;
        }

        public void Run(string url, string headerkey = null, string headervalue = null)
        {
            var request = _client.CreateRequest("Get", url);
            try
            {
                request.StateChanged += StateChanged;
                request.Done += Done;
                request.Aborted += Aborted;
                request.Error += Error;
                request.Timeout += Timeout;
                
                if(headerkey != null && headervalue != null)
                    request.SetHeader(headerkey, headervalue);
                
                request.SetResponseType(HttpResponseType.String);
                request.SendAsync();
            }
            catch(Exception e)
            {
                Update(request, "Exception", e.ToString());
            }
        }

        void StateChanged(HttpMessageHandlerRequest msg)
        {
            Update(msg);
        }

        void Done(HttpMessageHandlerRequest msg)
        {
            Update(msg, "Done");
        }

        void Error(HttpMessageHandlerRequest msg, string message)
        {
            Update(msg, "Error", message);
        }

        void Aborted(HttpMessageHandlerRequest msg)
        {
            Update(msg, "Aborted");
        }

        void Timeout(HttpMessageHandlerRequest msg)
        {
            Update(msg, "Timeout");
        }

        static string[] state = new []
        {
            "Unsent",
            "Opened",
            "Sent",
            "HeadersReceived",
            "Loading",
            "Done"
        };

        void Update(HttpMessageHandlerRequest msg, string eventType = null, string exception = null)
        {
            var data = new HttpEvent();

            if(eventType != null)
                data.Type = eventType;
            if(msg != null)
                data.Exception = exception;
            
            try
            {
                if(msg == null)
                    throw new ArgumentNullException("msg");

                data.StatusCode = msg.GetResponseStatus().ToString();
                data.State = state[msg.State];

                data.Headers = msg.GetResponseHeaders();
                data.Content = msg.GetResponseContentString();
            }
            catch(Exception e)
            {
                data.Exception = e.ToString();
            }

            _callback(data);
        }
    }

    public class HttpEvent
    {
        public string StatusCode { get; set; }

        public string Headers { get; set; }

        public string State { get; set; }

        public string Content { get; set; }

        public string Type { get; set; }
        
        public string Exception { get; set; }
    }
}
