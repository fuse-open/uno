using Uno;
using Uno.Collections;
using Uno.Diagnostics;
using Uno.Net.Http;

namespace Uno.Testing
{
    class HttpMessageDispatcher : ITestRunnerMessageDispatcher
    {
        private HashSet<HttpMessageHandlerRequest> _pendingRequests = new HashSet<HttpMessageHandlerRequest>();
        private HttpMessageHandler _handler;
        private bool _stopped;

        public void Start()
        {
            _handler = new HttpMessageHandler();
        }

        public void Stop()
        {
            lock (_pendingRequests)
            {
                _stopped = true;

                if (_pendingRequests.Count == 0)
                    Uno.Runtime.Implementation.Internal.Unsafe.Quit();
            }
        }

        public void Get(string uri)
        {
            var request = _handler.CreateRequest("GET", uri);
            request.Aborted += RemoveRequest;
            request.Error += OnError;
            request.Timeout += RemoveRequest;
            request.Done += RemoveRequest;
            request.SetResponseType(HttpResponseType.String);
            request.SendAsync();

            lock (_pendingRequests)
            {
                _pendingRequests.Add(request);
            }
        }

        private void RemoveRequest(HttpMessageHandlerRequest request)
        {
            lock (_pendingRequests)
            {
                _pendingRequests.Remove(request);

                if (_stopped && _pendingRequests.Count == 0)
                    Uno.Runtime.Implementation.Internal.Unsafe.Quit();
            }
        }

        private void OnError(HttpMessageHandlerRequest request, string message)
        {
            Debug.Log("Error sending request. " + message
                + ",Url=" + request.Url
                + ",Method=" + request.Method
                + ",ResponseStatus=" + request.GetResponseStatus() + ".");

            RemoveRequest(request);
        }

    }
}
