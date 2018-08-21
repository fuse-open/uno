using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Sockets
{
    [DotNetType("System.Net.Sockets.TcpListener")]
    public class TcpListener
    {
        Socket _serverSocket;
        readonly IPEndPoint _serverSocketEP;

        public EndPoint LocalEndpoint { get { return _serverSocket.IsBound ? _serverSocket.LocalEndPoint : _serverSocketEP; } }

        public TcpListener(IPEndPoint localEP)
        {
            if (localEP == null)
                throw new ArgumentNullException(nameof(localEP));

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocketEP = localEP;
        }

        public void Start(int backlog)
        {
            _serverSocket.Bind(_serverSocketEP);
            _serverSocket.Listen(backlog);
        }

        public void Stop()
        {
            _serverSocket.Close();
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Pending()
        {
            return _serverSocket.Poll(0, SelectMode.Read);
        }

        public Socket AcceptSocket()
        {
            return _serverSocket.Accept();
        }
    }
}
