using System.Net;
using System.Net.Sockets;

namespace Uno.TestRunner
{
    public static class HttpHelpers
    {
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
