using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Http.Implementation
{
    [DotNetType]
    public interface IHttpMessageHandlerRequestCallbacks
    {
        void OnAborted();
        void OnError(string errorMessage);
        void OnTimeout();
        void OnDone();
        void OnHeadersReceived();
        void OnProgress(int current, int total, bool hasTotal);
    }

    public class HttpMessageHandlerRequestCallbacks
        : IHttpMessageHandlerRequestCallbacks
    {
        HttpMessageHandlerRequest _request;

        public HttpMessageHandlerRequestCallbacks(
            HttpMessageHandlerRequest request)
        {
            _request = request;
        }

        public void OnAborted()
        {
            _request.OnAborted();
        }

        public void OnError(string errorMessage)
        {
            _request.OnError(errorMessage);
        }

        public void OnTimeout()
        {
            _request.OnTimeout();
        }

        public void OnDone()
        {
            _request.OnDone();
        }

        public void OnHeadersReceived()
        {
            _request.OnHeadersReceived();
        }

        public void OnProgress(int current, int total, bool hasTotal)
        {
            _request.OnProgress(current, total, hasTotal);
        }
    }

    [DotNetType]
    [TargetSpecificImplementation]
    public extern(CIL) static class CilHttpMessageHandler
    {
        [TargetSpecificImplementation]
        public extern static void ProcessEvents();
    }

    [DotNetType]
    [TargetSpecificImplementation]
    public extern(CIL) class CilHttpRequest : IHttpRequest
    {
        [TargetSpecificImplementation]
        public extern static CilHttpRequest Create(
            IHttpMessageHandlerRequestCallbacks requestCallbacks,
            string method, string url);

        [TargetSpecificImplementation] public extern void Dispose();

        [TargetSpecificImplementation] public extern void EnableCache(bool enableCache);

        [TargetSpecificImplementation] public extern void SetHeader(string name, string value);
        [TargetSpecificImplementation] public extern void SetTimeout(int timeoutInMilliseconds);

        [TargetSpecificImplementation] public extern void SendAsync(byte[] data);
        [TargetSpecificImplementation] public extern void SendAsync(string data);
        [TargetSpecificImplementation] public extern void SendAsync();

        [TargetSpecificImplementation] public extern void Abort();

        [TargetSpecificImplementation] public extern int GetResponseStatus();

        [TargetSpecificImplementation] public extern string GetResponseHeader(string name);
        [TargetSpecificImplementation] public extern string GetResponseHeaders();
        [TargetSpecificImplementation] public extern string GetResponseContentString();
        [TargetSpecificImplementation] public extern byte[] GetResponseContentByteArray();
    }
}
