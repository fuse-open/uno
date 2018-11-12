using Uno;
using Uno.Collections;
using Uno.Net.Http.Implementation;
using Uno.Threading;

namespace Uno.Net.Http
{
    public class HttpMessageHandler : IDisposable
    {
        internal static class StaticData
        {
            static object _syncLock = new object();
            static uint _totalPendingRequests;

            internal static void IncrementPendingRequests()
            {
                if defined(CIL || LINUX || MSVC)
                {
                    lock (_syncLock)
                    {
                        if (++_totalPendingRequests == 1)
                            Uno.Platform.Displays.MainDisplay.Tick += ProcessEvents;
                    }
                }
            }

            internal static void DecrementPendingRequests()
            {
                if defined(CIL || LINUX || MSVC)
                {
                    lock (_syncLock)
                    {
                        if (--_totalPendingRequests == 0)
                            Uno.Platform.Displays.MainDisplay.Tick -= ProcessEvents;
                    }
                }
            }

            internal static void ProcessEvents(object sender, Uno.Platform.TimerEventArgs args)
            {
                if defined(CIL)
                    CilHttpMessageHandler.ProcessEvents();
                if defined(LINUX || MSVC)
                    XliHttpMessageHandler.ProcessEvents();
            }
        }

        List<HttpMessageHandlerRequest> _pendingRequests;
        IDispatcher _defaultDispatcher;

        public HttpMessageHandler()
        {
            HttpMessageCache.Init();

            _pendingRequests = new List<HttpMessageHandlerRequest>();
            _defaultDispatcher = new HttpDefaultDispatcher();
        }

        //  NOTE: Finalizer does NOT need to call Dispose(), only externally
        //  referenced requests live on, at this point -- they're on their own.

        public void Dispose()
        {
            foreach (var request in _pendingRequests)
                if (request != null)
                    request.Dispose();

            _pendingRequests = null;
        }

        public void AbortPendingRequests()
        {
            foreach (var request in _pendingRequests)
                if (request != null)
                    request.Abort();
        }

        public HttpMessageHandlerRequest CreateRequest(string method, string url)
        {
            return CreateRequest(method, url, _defaultDispatcher);
        }

        public HttpMessageHandlerRequest CreateRequest(string method, string url, IDispatcher dispatcher)
        {
            StaticData.IncrementPendingRequests();

            var request = new HttpMessageHandlerRequest(this, method, url, dispatcher);

            for (int i = 0; i < _pendingRequests.Count; ++i)
            {
                if (_pendingRequests[i] == null)
                {
                    _pendingRequests[i] = request;
                    return request;
                }
            }

            _pendingRequests.Add(request);
            return request;
        }

        internal void CompleteRequest(HttpMessageHandlerRequest request)
        {
            // Minimize allocations by nulling request in array.
            for (int i = 0; i < _pendingRequests.Count; ++i)
            {
                if (_pendingRequests[i] == request)
                {
                    _pendingRequests[i] = null;
                    StaticData.DecrementPendingRequests();

                    break;
                }
            }
        }
    }
}
