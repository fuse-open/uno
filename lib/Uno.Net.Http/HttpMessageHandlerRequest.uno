using Uno;
using Uno.Collections;
using Uno.Net.Http.Implementation;
using Uno.Threading;

namespace Uno.Net.Http
{
    public class HttpMessageHandlerRequest : IDisposable
    {
        HttpMessageHandler _httpMessageHandler;
        IHttpRequest _httpRequest;
        IDispatcher _dispatcher;

        string _method;
        string _url;

        HttpRequestState _state;
        HttpResponseType _responseType;

        byte[] _optionalPayloadCache = null;

        internal HttpMessageHandlerRequest(HttpMessageHandler handler, string method, string url, IDispatcher dispatcher)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            if (method == null) throw new ArgumentNullException("method");
            if (url == null) throw new ArgumentNullException("url");
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");

            method = method.ToUpper();

            /*
            From: https://dvcs.w3.org/hg/xhr/raw-file/default/xhr-1/Overview.html
            If any code point in method is higher than U+00FF LATIN SMALL LETTER Y WITH DIAERESIS or after deflating method it does not match the Method token production, throw a "SyntaxError" exception and terminate these steps. Otherwise let method be the result of deflating method.
            If method is a case-insensitive match for CONNECT, DELETE, GET, HEAD, OPTIONS, POST, PUT, TRACE, or TRACK subtract 0x20 from each byte in the range 0x61 (ASCII a) to 0x7A (ASCII z).
            If it does not match any of the above, it is passed through literally, including in the final request.
            If method is a case-sensitive match for CONNECT, TRACE, or TRACK, throw a "SecurityError" exception and terminate these steps.
            */

            //TODO: Validate URL

            if defined(ANDROID)
                _httpRequest = new AndroidHttpRequest(this, method, url);
            if defined(LINUX || MSVC)
                _httpRequest = new XliHttpRequest(this, method, url);
            if defined(APPLE)
                _httpRequest = new iOSHttpRequest(this, method, url);

            if defined(CIL)
            {
                _httpRequest = CilHttpRequest.Create(
                    new HttpMessageHandlerRequestCallbacks(this), method, url);
            }

            _httpMessageHandler = handler;
            _method = method;
            _url = url;
            _dispatcher = dispatcher;

            State = HttpRequestState.Opened;
        }

        ~HttpMessageHandlerRequest()
        {
            if (_httpRequest == null)
                return;

            Dispose();
        }

        private void CheckDisposed()
        {
            if (_httpRequest == null)
                throw new ObjectDisposedException("HttpMessageHandlerRequest");
        }

        public void Dispose()
        {
            _httpRequest.Dispose();
            _httpRequest = null;

            if defined(CPLUSPLUS)
                _optionalPayloadCache = null;

            GC.SuppressFinalize(this);
        }

        internal void CompleteRequest()
        {
            _httpMessageHandler.CompleteRequest(this);
            _httpMessageHandler = null;
        }

        public string Method { get { return _method; } }
        public string Url { get { return _url; } }

        public HttpResponseType HttpResponseType
        {
            get { return _responseType; }
            set { SetResponseType(value); }
        }

        public HttpRequestState State
        {
            get { return _state; }
            private set
            {
                if (_state == value)
                    return;

                _state = value;
                OnStateChanged();
            }
        }

        internal void OnStateChanged()
        {
            // WARNING: MUST be run on uno thread

            // Errors have own hooks, don't trigger notifications here.
            if ((int)State > (int)HttpRequestState.Done)
                return;

            Action<HttpMessageHandlerRequest> handler = StateChanged;
            if (handler != null)
                handler(this);
        }

        public event Action<HttpMessageHandlerRequest> Aborted;
        public event Action<HttpMessageHandlerRequest, string> Error;
        public event Action<HttpMessageHandlerRequest> Timeout;
        public event Action<HttpMessageHandlerRequest> Done;
        public event Action<HttpMessageHandlerRequest> StateChanged;
        public event Action<HttpMessageHandlerRequest, int, int, bool> Progress;

        class DispatchClosure
        {
            Action<HttpMessageHandlerRequest> _action;
            HttpMessageHandlerRequest _arg;
            HttpRequestState _state;

            public DispatchClosure(HttpRequestState state, Action<HttpMessageHandlerRequest> action, HttpMessageHandlerRequest arg)
            {
                _action = action;
                _arg = arg;
                _state = state;
            }

            public void Run()
            {
                if(_arg.IsComplete())
                    return;
                _arg.State = _state;
                _action(_arg);
                _arg.CompleteRequest();
            }
        }

        class DispatchClosure<TArg1>
        {
            Action<HttpMessageHandlerRequest, TArg1> _action;
            HttpMessageHandlerRequest _arg0;
            TArg1 _arg1;
            HttpRequestState _state;

            public DispatchClosure(HttpRequestState state, Action<HttpMessageHandlerRequest, TArg1> action, HttpMessageHandlerRequest arg0, TArg1 arg1)
            {
                _action = action;
                _arg0 = arg0;
                _arg1 = arg1;
                _state = state;
            }

            public void Run()
            {
                if(_arg0.IsComplete())
                    return;
                _arg0.State = _state;
                _action(_arg0, _arg1);
                _arg0.CompleteRequest();
            }
        }

        class ProgressClosure
        {
            Action<HttpMessageHandlerRequest, int, int, bool> _action;
            HttpMessageHandlerRequest _request;
            int _current;
            int _total;
            bool _hasTotal;
            HttpRequestState _state;

            public ProgressClosure(HttpRequestState state, Action<HttpMessageHandlerRequest, int, int, bool> action, HttpMessageHandlerRequest request, int current, int total, bool hasTotal)
            {
                _action = action;
                _request = request;
                _current = current;
                _total = total;
                _hasTotal = hasTotal;
                _state = state;
            }

            public void Run()
            {
                if(_request.IsComplete())
                    return;
                    
                _request.State = _state;
                _action(_request, _current, _total, _hasTotal);
            }
        }

        bool IsComplete()
        {
            return (int)State >= (int)HttpRequestState.Done;
        }

        internal void OnAborted()
        {
            Action<HttpMessageHandlerRequest> handler = Aborted;
            if (handler != null)
                _dispatcher.Invoke(new DispatchClosure(HttpRequestState.Aborted, handler, this).Run);
        }

        internal void OnError(string platformspesificErrorMessage)
        {
            Action<HttpMessageHandlerRequest, string> handler = Error;
            if (handler != null)
                _dispatcher.Invoke(new DispatchClosure<string>(HttpRequestState.Errored, handler, this, platformspesificErrorMessage).Run);
        }

        internal void OnTimeout()
        {
            Action<HttpMessageHandlerRequest> handler = Timeout;
            if (handler != null)
                _dispatcher.Invoke(new DispatchClosure(HttpRequestState.TimedOut, handler, this).Run);
        }

        internal void OnDone()
        {
            Action<HttpMessageHandlerRequest> handler = Done;
            if (handler != null)
                _dispatcher.Invoke(new DispatchClosure(HttpRequestState.Done, handler, this).Run);
        }

        internal void OnHeadersReceived()
        {
            _dispatcher.Invoke(FireSetHeadersReceived);
        }
        private void FireSetHeadersReceived()
        {
            State = HttpRequestState.HeadersReceived;
        }

        internal void OnProgress(int current, int total, bool hasTotal)
        {
            Action<HttpMessageHandlerRequest, int, int, bool> handler = Progress;
            if (handler != null)
                _dispatcher.Invoke(new ProgressClosure(HttpRequestState.Loading, handler, this, current, total, hasTotal).Run);
        }

        public void EnableCache(bool enableCache)
        {
            CheckDisposed();

            if (State != HttpRequestState.Opened)
                throw new InvalidStateException();

            if defined(MAC && DesignMode) return; // Disable for mono

            _httpRequest.EnableCache(enableCache);
        }

        public void SetHeader(string name, string value)
        {
            CheckDisposed();

            if (State != HttpRequestState.Opened)
                throw new InvalidStateException();

            if (!IsHeaderValid(name, value))
                throw new Exception("Not allowed to set header \"" + name + "\" on this target.");

            _httpRequest.SetHeader(name, value);
        }

        public void SetTimeout(int timeoutInMilliseconds)
        {
            CheckDisposed();

            if (State != HttpRequestState.Opened)
                throw new InvalidStateException();

            _httpRequest.SetTimeout(timeoutInMilliseconds);
        }

        public void SetResponseType(HttpResponseType responseType)
        {
            CheckDisposed();

            if ((int)State >= (int)HttpRequestState.Loading)
                throw new InvalidStateException();

            _responseType = responseType;

            if defined(ANDROID)
                (_httpRequest as AndroidHttpRequest).SetResponseType(responseType);
        }

        public void SendAsync(byte[] data)
        {
            CheckDisposed();

            if(State != HttpRequestState.Opened)
                throw new InvalidStateException();

            // Change state now, but delay notification until SendAsync returns.
            _state = HttpRequestState.Sent;

            if (Method == "GET" || Method == "HEAD" || data == null || data.Length == 0)
                _httpRequest.SendAsync();
            else
            {
                // The following is used to protect c++ from the data being freed,
                // in c# and js this data is captured by the http request and so
                // it is safe during the transfer. This obviously isnt the case for
                // c++ and so rather than hack around with the refcounts in
                // implementation we use a field here.
                if defined(CPLUSPLUS)
                    _optionalPayloadCache = data;

                _httpRequest.SendAsync(data);
            }

            OnStateChanged();
        }

        public void SendAsync(string data)
        {
            CheckDisposed();

            if(State != HttpRequestState.Opened)
                throw new InvalidStateException();

            // Change state now, but delay notification until SendAsync returns.
            _state = HttpRequestState.Sent;

            if(Method == "GET" || Method == "HEAD" || data == null || data == "")
                _httpRequest.SendAsync();
            else
                _httpRequest.SendAsync(data);

            OnStateChanged();
        }

        public void SendAsync()
        {
            CheckDisposed();

            if(State != HttpRequestState.Opened)
                throw new InvalidStateException();

            // Change state now, but delay notification until SendAsync returns.
            _state = HttpRequestState.Sent;
            _httpRequest.SendAsync();
            OnStateChanged();
        }

        public void Abort()
        {
            CheckDisposed();

            if ((int)State >= (int)HttpRequestState.Done)
            {
                // Too late to Abort
                return;
            }

            _httpRequest.Abort();
            OnAborted();
        }

        public int GetResponseStatus()
        {
            CheckDisposed();

            if ((int)State < (int)HttpRequestState.HeadersReceived
                || (int)State > (int)HttpRequestState.Done)
                return 0;

            return _httpRequest.GetResponseStatus();
        }

        public string GetResponseHeader(string name)
        {
            CheckDisposed();

            if ((int)State < (int)HttpRequestState.HeadersReceived
                    || (int)State > (int)HttpRequestState.Done)
                return "";

            return _httpRequest.GetResponseHeader(name);
        }

        public string GetResponseHeaders()
        {
            CheckDisposed();

            if ((int)State < (int)HttpRequestState.HeadersReceived
                    || (int)State > (int)HttpRequestState.Done)
                return "";

            return _httpRequest.GetResponseHeaders();
        }

        public string GetResponseContentString()
        {
            CheckDisposed();

            if (HttpResponseType != HttpResponseType.String)
                throw new InvalidResponseTypeException();

            if ((int)State < (int)HttpRequestState.Loading
                    || (int)State > (int)HttpRequestState.Done)
                return "";

            return _httpRequest.GetResponseContentString() ?? "";
        }

        public byte[] GetResponseContentByteArray()
        {
            CheckDisposed();

            if (HttpResponseType != HttpResponseType.ByteArray)
                throw new InvalidResponseTypeException();

            if ((int)State < (int)HttpRequestState.Loading
                    || (int)State > (int)HttpRequestState.Done)
                return new byte[0];

            return _httpRequest.GetResponseContentByteArray()
                ?? new byte[0];
        }

        static bool IsHeaderValid(string name, string value)
        {
            /*
            TODO: Validate header according to http://www.rfc-editor.org/rfc/rfc7231.txt
            */
            return true;
        }
    }

    public sealed class InvalidResponseTypeException : Exception
    {
        public InvalidResponseTypeException() : base("Response type is invalid.")
        {}
    }

    public sealed class InvalidStateException : Exception
    {
        public InvalidStateException() : base("The object is in an invalid state.")
        {}

        public InvalidStateException(string message) : base(message)
        {}
    }
}
