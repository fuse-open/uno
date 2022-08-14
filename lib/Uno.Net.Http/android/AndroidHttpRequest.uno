using Uno;
using Uno.Collections;
using Uno.Net.Http;
using Android.com.fuse.ExperimentalHttp;
using Android.Base;
using Android.Base.Wrappers;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Http.Implementation
{
    internal extern(ANDROID) sealed class AndroidHttpRequest : Android.com.fuse.ExperimentalHttp.HttpRequest, IHttpRequest
    {
        HttpMessageHandlerRequest _request;
        List<byte[]> _responseData;
        byte[] _result;
        int _responseLength;

        internal AndroidHttpRequest(HttpMessageHandlerRequest request, string method, string url)
        :base (JNI.GetWrappedActivityObject(),
               JWrapper.Wrap(Android.Base.Types.String.UnoToJava(url)),
               JWrapper.Wrap(Android.Base.Types.String.UnoToJava(method)))
        {
            _request = request;
            _responseData = new List<byte[]>();
            _responseLength = 0;
            _result = new byte[0];
        }

        public void Dispose()
        {
            // ?
        }

        public void EnableCache(bool enableCache)
        {
            SetCaching(enableCache);
        }

        public void SetHeader(string name, string value)
        {
            var jName = JWrapper.Wrap(Android.Base.Types.String.UnoToJava(name));
            var jValue = JWrapper.Wrap(Android.Base.Types.String.UnoToJava(value));
            SetHeader(jName, jValue);
        }

        internal void SetResponseType(HttpResponseType responseType)
        {
            SetResponseType((int)responseType);
        }

        public void SendAsync(byte[] data)
        {
            var tmp = JWrapper.Wrap(Android.Base.Types.ByteBuffer.NewDirectByteBuffer(data));
            SendAsyncBuf(tmp);
        }

        public void SendAsync(string data)
        {
            var jData = JWrapper.Wrap(Android.Base.Types.String.UnoToJava(data));;
            SendAsyncStr(jData);
        }

        public string GetResponseHeader(string name)
        {
            var jName = JWrapper.Wrap(Android.Base.Types.String.UnoToJava(name));
            return Android.Base.Types.String.JavaToUno(GetResponseHeader(jName));
        }

        public new string GetResponseHeaders()
        {
            return Android.Base.Types.String.JavaToUno(base.GetResponseHeaders());
        }

        public string GetResponseContentString()
        {
            return Android.Base.Types.String.JavaToUno(GetResponseString());
        }

        public byte[] GetResponseContentByteArray()
        {
            return _result;
        }

        public override void OnAborted()
        {
            _request.OnAborted();
        }

        public override void OnTimeout()
        {
            _request.OnTimeout();
        }

        public override void OnDone()
        {
            _request.OnDone();
        }

        public override void OnHeadersReceived()
        {
            _request.OnHeadersReceived();
        }

        public override void OnProgress(int current, int total, bool hasTotal)
        {
            _request.OnProgress(current, total, hasTotal);
        }

        public override void OnDataReceived(IJWrapper arg0, int arg1)
        {
            if (arg1 == -1) {
                if (_responseLength==0) return;

                _result = new byte[_responseLength];
                int pos = 0;
                foreach (var chunk in _responseData) {
                    var chunkLength = chunk.Length;
                    Array.Copy(chunk, 0, _result, pos, chunkLength);
                    pos += chunkLength;
                }
            } else {
                // still downloading
                _responseData.Add(Android.Base.Types.Arrays.JavaToUnoByteArray(arg0._GetJavaObject(), arg1));
                _responseLength+=(int)arg1;
            }
        }

        public override void OnError(IJWrapper arg0)
        {
            _request.OnError(Android.Base.Types.String.JavaToUno(arg0));
        }
    }
}
