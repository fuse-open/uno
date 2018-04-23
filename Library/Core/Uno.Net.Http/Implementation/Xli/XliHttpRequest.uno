using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Http.Implementation
{
    [TargetSpecificType]
    extern(LINUX || MSVC12) struct XliHttpClientHandle
    {
    }

    [TargetSpecificType]
    extern(LINUX || MSVC12) struct XliHttpRequestHandle
    {
    }

    [TargetSpecificImplementation]
    internal extern(LINUX || MSVC12) static class XliHttpMessageHandler
    {
        internal static XliHttpClientHandle _clientHandle;

        static XliHttpMessageHandler()
        {
            _clientHandle = extern<XliHttpClientHandle>
                "::Xli::HttpClient::Create()";
            extern(_clientHandle)
                "$0->SetEventHandler(new uXliHttpEventHandler())";
        }

        internal static void ProcessEvents()
        {
            extern(_clientHandle) "$0->Update()";
        }
    }

    [TargetSpecificImplementation]
    internal extern(LINUX || MSVC12) class XliHttpRequest : IHttpRequest
    {
        HttpMessageHandlerRequest _request;
        XliHttpRequestHandle _requestHandle;

        internal XliHttpRequest(
            HttpMessageHandlerRequest request, string method, string url)
        {
            _request = request; // Retain request object so it's not deleted in the process
            _requestHandle = extern<XliHttpRequestHandle>(
                    XliHttpMessageHandler._clientHandle, method, url)
                "$0->CreateRequest(uStringToXliString($1), uStringToXliString($2))";

            extern(_requestHandle, request) "$0->SetUserData($1)";
        }

        public void Dispose()
        {
            extern(_requestHandle) "$0->Release()";
            _requestHandle = extern<XliHttpRequestHandle> "NULL";
            _request = null; // AutoRelease request object so it's deleted later
        }

        public void EnableCache(bool enableCache)
        {
            // TODO
        }

        public void SetHeader(string name, string value)
        {
            extern(_requestHandle, name, value)
                "$0->SetHeader(uStringToXliString($1), uStringToXliString($2))";
        }

        public void SetTimeout(int timeoutInMilliseconds)
        {
            extern(_requestHandle, timeoutInMilliseconds)
                "$0->SetTimeout($1)";
        }

        public void SendAsync(byte[] data)
        {
            extern(_requestHandle, data)
                "$0->SendAsync($1->Ptr(), $1->Length())";
        }

        public void SendAsync(string data)
        {
            extern(_requestHandle, data)
                "$0->SendAsync(uStringToXliString($1))";
        }

        public void SendAsync()
        {
            extern(_requestHandle) "$0->SendAsync()";
        }

        public void Abort()
        {
            extern(_requestHandle) "$0->Abort()";
        }

        public int GetResponseStatus()
        {
            return extern<int>(_requestHandle) "$0->GetResponseStatus()";
        }

        [TargetSpecificImplementation] public extern string GetResponseHeader(string name);
        [TargetSpecificImplementation] public extern string GetResponseHeaders();

        public string GetResponseContentString()
        {
            extern(_requestHandle) "uBase::DataAccessor* body = $0->GetResponseBody()";
            return extern<string> "uString::Utf8((const char*)body->GetPtr(), (int)body->GetSizeInBytes())";
        }

        public byte[] GetResponseContentByteArray()
        {
            extern(_requestHandle) "uBase::DataAccessor* body = $0->GetResponseBody()";
            return extern<byte[]>(typeof(byte)) "uArray::New($0->Array(), (int)body->GetSizeInBytes(), body->GetPtr())";
        }
    }
}
