using Uno;
using Uno.IO;
using Uno.Testing;
using Uno.Net.Sockets;

namespace Uno.Net.Sockets.Test
{
    public class TcpListenerTest
    {
        void CreateNull()
        {
            var sock = new TcpListener(null);
        }

        [Test]
        public void Create()
        {
            Assert.Throws<ArgumentNullException>(CreateNull);

            new TcpListener(new IPEndPoint(IPAddress.Any, 1337));
        }

        public void Start()
        {
            var tcpListener = new TcpListener(new IPEndPoint(IPAddress.Loopback, 0));

            Assert.AreEqual(0, ((IPEndPoint)tcpListener.LocalEndpoint).Port);
            tcpListener.Start(1);

            Assert.AreNotEqual(null, tcpListener.LocalEndpoint);
            Assert.AreEqual(true, tcpListener.LocalEndpoint is IPEndPoint);
            Assert.AreNotEqual(0, ((IPEndPoint)tcpListener.LocalEndpoint).Port);
        }

        public void Stop()
        {
            var tcpListener = new TcpListener(new IPEndPoint(IPAddress.Loopback, 0));

            Assert.AreEqual(0, ((IPEndPoint)tcpListener.LocalEndpoint).Port);
            tcpListener.Start(1);
            Assert.AreNotEqual(0, ((IPEndPoint)tcpListener.LocalEndpoint).Port);
            tcpListener.Stop();
            Assert.AreEqual(0, ((IPEndPoint)tcpListener.LocalEndpoint).Port);
        }

        public void AcceptSocket()
        {
            var tcpListener = new TcpListener(new IPEndPoint(IPAddress.Loopback, 0));
            tcpListener.Start(1);

            var clientSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(tcpListener.LocalEndpoint);

            var serverSocket = tcpListener.AcceptSocket();
            Assert.AreEqual(serverSocket.LocalEndPoint, clientSocket.RemoteEndPoint);
            Assert.AreEqual(serverSocket.RemoteEndPoint, clientSocket.LocalEndPoint);
        }

        public void Pending()
        {
            var tcpListener = new TcpListener(new IPEndPoint(IPAddress.Loopback, 0));
            tcpListener.Start(1);
            Assert.AreEqual(false, tcpListener.Pending());

            var clientSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(tcpListener.LocalEndpoint);

            Assert.AreEqual(true, tcpListener.Pending());
            tcpListener.AcceptSocket();
            Assert.AreEqual(false, tcpListener.Pending());
        }
    }
}
