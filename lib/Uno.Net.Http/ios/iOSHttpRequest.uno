using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Http.Implementation
{
    [TargetSpecificType]
    extern(APPLE) struct iOSHttpRequestHandle
    {
    }

    [TargetSpecificImplementation]
    internal extern(APPLE) class iOSHttpRequest : IHttpRequest
    {
        iOSHttpRequestHandle _requestHandle;

        internal iOSHttpRequest(
            HttpMessageHandlerRequest request, string method, string url)
        {
            _requestHandle = extern<iOSHttpRequestHandle>(request, method, url)
                "new ::Uno::Net::Http::iOS::HttpRequest($0, $1, $2)";
        }
        public void Dispose()
        {
            extern(_requestHandle) "delete $0";
            _requestHandle = extern<iOSHttpRequestHandle> "nullptr";
        }

        public void EnableCache(bool enableCache)
        {
            extern(_requestHandle, enableCache) "$0->SetCacheEnabled($1)";
        }

        public void SetHeader(string name, string value)
        {
            extern(_requestHandle, name, value) "$0->SetHeader($1, $2)";
        }

        public void SetTimeout(int timeoutInMilliseconds)
        {
            extern(_requestHandle, timeoutInMilliseconds) "$0->SetTimeout($1)";
        }

        public void SendAsync(byte[] data)
        {
            extern(_requestHandle, data)
                "$0->SendAsync($1->Ptr(), $1->Length())";
        }

        public void SendAsync(string data)
        {
            extern(_requestHandle, data) "$0->SendAsync($1)";
        }

        public void SendAsync()
        {
            extern(_requestHandle) "$0->SendAsync(nullptr, 0)";
        }

        public void Abort()
        {
            extern(_requestHandle) "$0->Abort()";
        }

        public int GetResponseStatus()
        {
            return extern<int>(_requestHandle) "$0->GetResponseStatus()";
        }

        public string GetResponseHeader(string name)
        {
            return extern<string>(_requestHandle, name)
                "$0->GetResponseHeader($1)";
        }

        public string GetResponseHeaders()
        {
            return extern<string>(_requestHandle) "$0->GetResponseHeaders()";
        }

        public string GetResponseContentString()
        {
            return extern<string>(_requestHandle)
                "$0->GetResponseContentString()";
        }

        public byte[] GetResponseContentByteArray()
        {
            return extern<byte[]>(_requestHandle)
                "$0->GetResponseContentByteArray()";
        }
    }
}
