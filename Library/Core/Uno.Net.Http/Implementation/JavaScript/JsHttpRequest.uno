using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Http.Implementation
{
    [TargetSpecificType]
    internal extern(JAVASCRIPT) struct JsHttpRequestHandle
    {
    }

    [TargetSpecificImplementation]
    internal extern(JAVASCRIPT) class JsHttpRequest : IHttpRequest
    {
        JsHttpRequestHandle _requestHandle;

        internal JsHttpRequest(
            HttpMessageHandlerRequest request, string method, string url)
        {
            _requestHandle = CreateHandle(request, method, url);
        }

        [TargetSpecificImplementation]
        static extern JsHttpRequestHandle CreateHandle(
            HttpMessageHandlerRequest request, string method, string url);

        public void Dispose()
        {
            _requestHandle = extern<JsHttpRequestHandle> "null";
        }

        public void EnableCache(bool enableCache)
        {
            // TODO
        }

        public void SetHeader(string name, string value)
        {
            extern(_requestHandle, name, value) "$0.setRequestHeader($1, $2)";
        }

        public void SetTimeout(int timeoutInMilliseconds)
        {
            extern(_requestHandle, timeoutInMilliseconds) "$0.timeout = $1";
        }

        internal void SetResponseType(HttpResponseType responseType)
        {
            if (responseType == HttpResponseType.ByteArray)
                extern(_requestHandle) "$0.responseType = \"arraybuffer\"";
            else
                extern(_requestHandle) "$0.responseType = \"text\"";
        }

        public void SendAsync(byte[] data)
        {
            extern(_requestHandle, data) "$0.send($1)";
        }

        public void SendAsync(string data)
        {
            extern(_requestHandle, data) "$0.send($1)";
        }

        public void SendAsync()
        {
            extern(_requestHandle) "$0.send()";
        }

        public void Abort()
        {
            extern(_requestHandle) "$0.abort()";
        }

        public int GetResponseStatus()
        {
            return extern<int>(_requestHandle) "$0.status";
        }

        public string GetResponseHeader(string name)
        {
            return extern<string>(_requestHandle, name)
                "$0.getResponseHeader($1)";
        }
        public string GetResponseHeaders()
        {
            return extern<string>(_requestHandle) "$0.getAllResponseHeaders()";
        }
        public string GetResponseContentString()
        {
            return extern<string>(_requestHandle) "$0.responseText";
        }
        public byte[] GetResponseContentByteArray()
        {
            return extern<byte[]>(_requestHandle) "new Uint8Array($0.response)";
        }
    }
}
