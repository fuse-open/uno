using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Http.Implementation
{
    [DotNetType]
    public interface IHttpRequest : IDisposable
    {
        void EnableCache(bool enableCache);

        void SetHeader(string name, string value);
        void SetTimeout(int timeoutInMilliseconds);

        void SendAsync(byte[] data);
        void SendAsync(string data);
        void SendAsync();

        void Abort();

        int GetResponseStatus();

        string GetResponseHeader(string name);
        string GetResponseHeaders();
        string GetResponseContentString();
        byte[] GetResponseContentByteArray();
    }
}
