using Uno;
using Uno.Compiler;
using Uno.IO;
using Uno.Testing;
using Uno.Net.Sockets;

namespace Uno.Net.Sockets.Test
{
    static class EndPointAssert
    {
        public static void AssertValid(this EndPoint endpoint,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            var ipEndPoint = (IPEndPoint)endpoint;
            Assert.AreNotEqual(null, ipEndPoint);
            Assert.AreNotEqual(IPAddress.IPv6None, ipEndPoint.Address);
            Assert.AreNotEqual(IPAddress.Any, ipEndPoint.Address);
        }
    }

    public class SocketTest
    {
        [Test]
        public void Create()
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.AreEqual(0, sock.Available);
            Assert.AreEqual(false, sock.Connected);
            Assert.AreEqual(null, sock.LocalEndPoint);
            Assert.AreEqual(null, sock.RemoteEndPoint);
        }

        [Test]
        public void ListenAndConnectIPv4()
        {
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.AreEqual(null, listener.LocalEndPoint);
            Assert.AreEqual(false, listener.IsBound);

            listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            listener.LocalEndPoint.AssertValid();
            Assert.AreNotEqual(0, ((IPEndPoint)listener.LocalEndPoint).Port);
            Assert.AreEqual(true, listener.IsBound);

            listener.Listen(1);

            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.AreEqual(false, serverSocket.Connected);
            Assert.AreEqual(null, serverSocket.LocalEndPoint);
            Assert.AreEqual(false, serverSocket.IsBound);

            serverSocket.Connect(listener.LocalEndPoint);
            Assert.AreEqual(true, serverSocket.Connected);
            serverSocket.LocalEndPoint.AssertValid();
            serverSocket.RemoteEndPoint.AssertValid();
            Assert.AreEqual(true, serverSocket.IsBound);

            var clientSocket = listener.Accept();
            Assert.AreEqual(true, clientSocket.Connected);
            clientSocket.LocalEndPoint.AssertValid();
            clientSocket.RemoteEndPoint.AssertValid();
            listener.Close();
        }

        [Test, Ignore("Throws exception on Linux.", "LINUX")]
        public void ListenAndConnectIPv6()
        {
            var listener = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            Assert.AreEqual(null, listener.LocalEndPoint);
            Assert.AreEqual(false, listener.IsBound);

            listener.Bind(new IPEndPoint(IPAddress.IPv6Loopback, 0));
            listener.LocalEndPoint.AssertValid();
            Assert.AreNotEqual(0, ((IPEndPoint)listener.LocalEndPoint).Port);
            Assert.AreEqual(true, listener.IsBound);

            listener.Listen(1);

            var serverSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            Assert.AreEqual(false, serverSocket.Connected);
            Assert.AreEqual(null, serverSocket.LocalEndPoint);
            Assert.AreEqual(false, serverSocket.IsBound);

            serverSocket.Connect(listener.LocalEndPoint);
            Assert.AreEqual(true, serverSocket.Connected);
            serverSocket.LocalEndPoint.AssertValid();
            serverSocket.RemoteEndPoint.AssertValid();
            Assert.AreEqual(true, serverSocket.IsBound);

            var clientSocket = listener.Accept();
            Assert.AreEqual(true, clientSocket.Connected);
            clientSocket.LocalEndPoint.AssertValid();
            clientSocket.RemoteEndPoint.AssertValid();
            listener.Close();
        }

        public void BindAndReadRemoteEndPoint()
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            Assert.AreEqual(null, sock.RemoteEndPoint);
        }

        [Test]
        public void RemoteEndPoint()
        {
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.AreEqual(null, listener.RemoteEndPoint);

            listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            listener.Listen(1);

            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.AreEqual(null, serverSocket.RemoteEndPoint);

            serverSocket.Connect(listener.LocalEndPoint);
            Assert.AreNotEqual(null, serverSocket.RemoteEndPoint);

            var clientSocket = listener.Accept();
            Assert.AreNotEqual(null, clientSocket.RemoteEndPoint);

            clientSocket.Shutdown(SocketShutdown.Both);

            // https://github.com/fusetools/uno/issues/1519
            if defined (!CIL || !HOST_OSX)
                Assert.AreNotEqual(null, clientSocket.RemoteEndPoint);

            var serverSocket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket2.Connect(listener.LocalEndPoint);
            var clientSocket2 = listener.Accept();
            clientSocket2.Shutdown(SocketShutdown.Both);

            // https://github.com/fusetools/uno/issues/1519
            if defined (!CIL || !HOST_OSX)
                Assert.AreNotEqual(null, clientSocket2.RemoteEndPoint);

            listener.Close();

            // https://github.com/fusetools/uno/issues/1519
            if defined (!CIL || !HOST_OSX)
                Assert.Throws<SocketException>(BindAndReadRemoteEndPoint);
        }

        void SetupSocketPair(out Socket serverSocket, out Socket clientSocket)
        {
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            listener.Listen(1);

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(listener.LocalEndPoint);

            clientSocket = listener.Accept();
            listener.Close();
        }

        [Test]
        public void Listen()
        {
            Socket serverSocket, clientSocket;
            SetupSocketPair(out serverSocket, out clientSocket);

            var clientSocketStream = new NetworkStream(clientSocket);
            var clientSocketReader = new StreamReader(clientSocketStream);
            var clientSocketWriter = new StreamWriter(clientSocketStream);

            var serverSocketStream = new NetworkStream(serverSocket);
            var serverSocketWriter = new StreamWriter(serverSocketStream);
            var serverSocketReader = new StreamReader(serverSocketStream);

            serverSocketWriter.Write("hello server!\n");
            serverSocketWriter.Flush();
            Assert.AreEqual("hello server!",  clientSocketReader.ReadLine());

            clientSocketWriter.Write("hello client!\n");
            clientSocketWriter.Flush();
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            var hello = serverSocketReader.ReadLine();
            Assert.AreEqual("hello client!", hello);

            // https://github.com/fusetools/uno/issues/1519
            if defined (!CIL || !HOST_OSX)
                serverSocket.Shutdown(SocketShutdown.Both);

            serverSocket.Close();
        }

        [Test]
        public void Poll()
        {
            Socket serverSocket, clientSocket;
            SetupSocketPair(out serverSocket, out clientSocket);

            Assert.AreEqual(false, serverSocket.Poll(0, SelectMode.Read));

            int timeout = 250000;

            var startTime = Uno.Diagnostics.Clock.GetSeconds();
            Assert.AreEqual(false, serverSocket.Poll(timeout, SelectMode.Read));
            var duration = (Uno.Diagnostics.Clock.GetSeconds() - startTime) * 1e6;
            Assert.AreEqual((double)timeout, (double)duration, timeout * 0.5);

            clientSocket.Send(new byte[] {1, 2, 3});
            Assert.AreEqual(true, serverSocket.Poll(100000, SelectMode.Read));

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            // https://github.com/fusetools/uno/issues/1519
            if defined (!CIL || !HOST_OSX)
                serverSocket.Shutdown(SocketShutdown.Both);

            serverSocket.Close();
        }

        [Test]
        public void Send()
        {
            Socket serverSocket, clientSocket;
            SetupSocketPair(out serverSocket, out clientSocket);
            var serverSocketStream = new NetworkStream(serverSocket);
            var serverSocketReader = new BinaryReader(serverSocketStream);

            clientSocket.Send(new byte[] {1, 2, 3}, 1, 1, SocketFlags.None);
            Assert.AreEqual(true, serverSocket.Poll(100, SelectMode.Read));
            Assert.AreEqual(2, serverSocketReader.ReadByte());
            Assert.AreEqual(false, serverSocket.Poll(100, SelectMode.Read));

            clientSocket.Send(new byte[] {1, 2, 3});
            Assert.AreEqual(true, serverSocket.Poll(1000, SelectMode.Read));
            Assert.AreEqual(1, serverSocketReader.ReadByte());
            Assert.AreEqual(true, serverSocket.Poll(100, SelectMode.Read));
            Assert.AreEqual(2, serverSocketReader.ReadByte());
            Assert.AreEqual(true, serverSocket.Poll(100, SelectMode.Read));
            Assert.AreEqual(3, serverSocketReader.ReadByte());
            Assert.AreEqual(false, serverSocket.Poll(100, SelectMode.Read));
        }

        // TODO: add a test for Socket.Receive()

        void ConnectToLocalHost()
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect("127.0.0.1", 1337);
        }

        [Test]
        public void Connect()
        {
            Assert.Throws<SocketException>(ConnectToLocalHost);
        }

        void ShutdownNonConnected()
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Shutdown(SocketShutdown.Both);
        }

        [Test]
        public void Shutdown()
        {
            Assert.Throws<SocketException>(ShutdownNonConnected);
        }
    }
}
