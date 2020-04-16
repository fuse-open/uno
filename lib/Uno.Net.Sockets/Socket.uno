using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Net;

namespace Uno.Net.Sockets
{
    [DotNetType("System.Net.Sockets.AddressFamily")]
    public enum AddressFamily
    {
        InterNetwork = 2,
        InterNetworkV6 = 23,
    }

    [DotNetType("System.Net.Sockets.SocketType")]
    public enum SocketType
    {
        Stream = 1,
    }

    [DotNetType("System.Net.Sockets.ProtocolType")]
    public enum ProtocolType
    {
        Tcp = 6,
    }

    [DotNetType("System.Net.Sockets.SelectMode")]
    public enum SelectMode
    {
        Read = 0,
        Write = 1,
        Error = 2
    }

    [DotNetType("System.Net.Sockets.SocketShutdown")]
    public enum SocketShutdown
    {
        Receive = 0,
        Send = 1,
        Both = 2
    }

    [FlagsAttribute]
    [DotNetType("System.Net.Sockets.SocketFlags")]
    public enum SocketFlags
    {
        None = 0
    }

    [extern(UNIX) Require("Source.Include", "sys/socket.h")]
    [extern(MSVC) Require("Source.Declaration", "typedef ULONG in_addr_t;")]
    [extern(MSVC) Require("Source.Declaration", "#define SHUT_RD SD_RECEIVE")]
    [extern(MSVC) Require("Source.Declaration", "#define SHUT_WR SD_SEND")]
    [extern(MSVC) Require("Source.Declaration", "#define SHUT_RDWR SD_BOTH")]
    [extern(UNIX) Require("Source.Include", "errno.h")]
    extern(CPLUSPLUS) internal class SocketHelpers
    {
        public static int GetFamily(AddressFamily addressFamily)
        @{
            switch (addressFamily)
            {
                case @{AddressFamily.InterNetwork}: return AF_INET;
                case @{AddressFamily.InterNetworkV6}: return AF_INET6;
                default: U_THROW(@{Uno.Exception(string):New(uString::Utf8("Invalid value for AddressFamily"))});
            }
        @}

        public static int GetType(SocketType socketType)
        @{
            switch (socketType)
            {
                case @{SocketType.Stream}: return SOCK_STREAM;
                default: U_THROW(@{Uno.Exception(string):New(uString::Utf8("Invalid value for SocketType"))});
            }
        @}

        public static int GetProtocol(ProtocolType protocolType)
        @{
            switch (protocolType)
            {
                case @{ProtocolType.Tcp}: return IPPROTO_TCP;
                default: U_THROW(@{Uno.Exception(string):New(uString::Utf8("Invalid value for ProtocolType"))});
            }
        @}

        public static int GetSocketShudown(SocketShutdown socketShutdown)
        @{
            switch (socketShutdown)
            {
                case @{SocketShutdown.Receive}: return SHUT_RD;
                case @{SocketShutdown.Send}: return SHUT_WR;
                case @{SocketShutdown.Both}: return SHUT_RDWR;
                default: U_THROW(@{Uno.Exception(string):New(uString::Utf8("Invalid value for SocketShutdown"))});
            }
        @}

        public static IPEndPoint GetLocalEndPoint(Socket.SocketHandle sock)
        @{
            sockaddr_storage ss = { 0 };
            socklen_t len = sizeof(ss);

            int result = getsockname(sock, (sockaddr *)&ss, &len);
            if (result < 0)
                return nullptr;

            sockaddr_in *sa = (sockaddr_in *)&ss;
            @{IPAddress} ipAddress;
            if (sa->sin_family == AF_INET)
            {
                @{byte[]} tmp = uArray::New(@{byte[]:TypeOf}, 4, &sa->sin_addr.s_addr);
                ipAddress = @{IPAddress(byte[]):New(tmp)};
            }
            else
            {
                sockaddr_in6 *sa6 = (sockaddr_in6 *)&ss;
                @{byte[]} tmp = uArray::New(@{byte[]:TypeOf}, 16, &sa6->sin6_addr);
                ipAddress = @{IPAddress(byte[], long):New(tmp, sa6->sin6_scope_id)};
            }
            return @{IPEndPoint(IPAddress, int):New(ipAddress, ntohs(sa->sin_port))};
        @}

        public static IPEndPoint GetRemoteEndPoint(Socket.SocketHandle sock)
        @{
            sockaddr_storage ss = { 0 };
            socklen_t len = sizeof(ss);

            int result = getpeername(sock, (sockaddr *)&ss, &len);
            if (result < 0)
                return nullptr;

            sockaddr_in *sa = (sockaddr_in *)&ss;
            @{IPAddress} ipAddress;
            if (sa->sin_family == AF_INET)
            {
                @{byte[]} tmp = uArray::New(@{byte[]:TypeOf}, 4, &sa->sin_addr.s_addr);
                ipAddress = @{IPAddress(byte[]):New(tmp)};
            }
            else
            {
                sockaddr_in6 *sa6 = (sockaddr_in6 *)&ss;
                @{byte[]} tmp = uArray::New(@{byte[]:TypeOf}, 16, &sa6->sin6_addr);
                ipAddress = @{IPAddress(byte[], long):New(tmp, sa6->sin6_scope_id)};
            }
            return @{IPEndPoint(IPAddress, int):New(ipAddress, ntohs(sa->sin_port))};
        @}

        public static int Connect(Socket.SocketHandle sock, int family, byte[] address, long scopeId, int port)
        @{
            sockaddr_storage ss = { 0 };
            socklen_t size;

            if (family == AF_INET)
            {
                sockaddr_in *sa = (sockaddr_in *)&ss;
                size = sizeof(*sa);
                sa->sin_family = family;
                sa->sin_port = htons(port);
                memcpy(&sa->sin_addr.s_addr, address->Ptr(), 4);
            }
            else
            {
                sockaddr_in6 *sa = (sockaddr_in6 *)&ss;
                size = sizeof(*sa);
                sa->sin6_family = family;
                sa->sin6_port = htons(port);
                memcpy(&sa->sin6_addr, address->Ptr(), 16);
                sa->sin6_scope_id = (unsigned long)scopeId;
            }

            return connect(sock, (sockaddr *)&ss, size);
        @}

        public static int Bind(Socket.SocketHandle sock, int family, byte[] address, long scopeId, int port)
        @{
            sockaddr_storage ss = { 0 };
            socklen_t size;

            if (family == AF_INET)
            {
                sockaddr_in *sa = (sockaddr_in *)&ss;
                size = sizeof(*sa);
                sa->sin_family = family;
                sa->sin_port = htons(port);
                memcpy(&sa->sin_addr.s_addr, address->Ptr(), 4);
            }
            else
            {
                sockaddr_in6 *sa = (sockaddr_in6 *)&ss;
                size = sizeof(*sa);
                sa->sin6_family = family;
                sa->sin6_port = htons(port);
                memcpy(&sa->sin6_addr, address->Ptr(), 16);
                sa->sin6_scope_id = (unsigned long)scopeId;
            }

            return bind(sock, (sockaddr *)&ss, size);
        @}

        public static int Poll(Socket.SocketHandle sock, int milliseconds, SelectMode mode)
        @{
            timeval timeout = { 0 };
            timeout.tv_usec = milliseconds % 1000000;
            timeout.tv_sec = milliseconds / 1000000;

            fd_set fds;
            FD_ZERO(&fds);
            FD_SET($0, &fds);

            switch (mode)
            {
                case @{SelectMode.Read}:  return select((int)sock + 1, &fds, nullptr, nullptr, &timeout);
                case @{SelectMode.Write}: return select((int)sock + 1, nullptr, &fds, nullptr, &timeout);
                case @{SelectMode.Error}: return select((int)sock + 1, nullptr, nullptr, &fds, &timeout);
                default: U_THROW(@{Uno.Exception(string):New(uString::Utf8("Invalid value for ProtocolType"))});
            }
        @}

        public static Socket.SocketHandle Accept(Socket.SocketHandle sock)
        @{
            return accept(sock, nullptr, nullptr);
        @}

        extern(MSVC) public static int Ioctl(Socket.SocketHandle sock, int request, out int arg)
        @{
            u_long tmp;
            int ret = ioctlsocket(sock, (long)request, &tmp);
            *arg = tmp;
            return ret;
        @}

        extern(UNIX) public static int Ioctl(Socket.SocketHandle sock, int request, out int arg)
        @{
            return ioctl(sock, request, arg);
        @}

        extern(MSVC) public static int Shutdown(Socket.SocketHandle sock, int how)
        @{
            int result = shutdown(sock, how);
            if (result < 0 && WSAGetLastError() == WSAENOTCONN)
                return 0;
            return result;
        @}

        extern(UNIX) public static int Shutdown(Socket.SocketHandle sock, int how)
        @{
            int result = shutdown(sock, how);
            if (result < 0 && errno == ENOTCONN)
                return 0;
            return result;
        @}
    }

    [DotNetType("System.Net.Sockets.Socket")]
    [extern(MSVC) Require("Header.Include", "ws2tcpip.h")]
    [extern(MSVC) Require("LinkLibrary", "ws2_32")]
    [extern(ANDROID) Require("Source.Include", "arpa/inet.h")]
    [extern(UNIX) Require("Source.Include", "netdb.h")]
    [extern(UNIX) Require("Source.Include", "netinet/in.h")]
    [extern(UNIX) Require("Source.Include", "sys/ioctl.h")]
    [extern(UNIX) Require("Source.Include", "sys/socket.h")]
    [extern(UNIX) Require("Source.Include", "sys/types.h")]
    [extern(UNIX) Require("Source.Include", "unistd.h")]
    public class Socket : IDisposable
    {
        [TargetSpecificType]
        [extern(MSVC) Set("TypeName", "SOCKET")]
        [extern(MSVC) Set("DefaultValue", "INVALID_SOCKET")]
        [extern(CPLUSPLUS && UNIX) Set("TypeName", "int")]
        [extern(CPLUSPLUS && UNIX) Set("DefaultValue", "-1")]
        extern(CPLUSPLUS) internal struct SocketHandle
        {
            public static readonly SocketHandle Invalid;

            public static bool operator == (SocketHandle left, SocketHandle right)
            {
                return extern<bool> "$0 == $1";
            }

            public static bool operator != (SocketHandle left, SocketHandle right)
            {
                return extern<bool> "$0 != $1";
            }
        }

        extern(CPLUSPLUS) SocketHandle _handle;

        public Socket(
            AddressFamily addressFamily,
            SocketType socketType,
            ProtocolType protocolType)
        {
            if defined(CPLUSPLUS)
            {
                if defined(MSVC)
                    NetworkHelpers.EnsureWinsockInitialized();

                var family = SocketHelpers.GetFamily(addressFamily);
                var type = SocketHelpers.GetType(socketType);
                var protocol = SocketHelpers.GetProtocol(protocolType);

                _handle = extern<SocketHandle>(family, type, protocol)"socket($0, $1, $2)";
                if (_handle == SocketHandle.Invalid)
                    throw new Exception(NetworkHelpers.GetError());
            }
        }

        extern(CPLUSPLUS) private Socket(SocketHandle handle, bool connected)
        {
            _handle = handle;
            _connected = connected;

            if (connected)
            {
                UpdateLocalEndPoint();
                UpdateRemoteEndPoint();
            }
        }

        bool _connected;
        public bool Connected { get { return _connected; } }

        public int Available
        {
            get
            {
                if defined(CPLUSPLUS)
                {
                    int available;
                    int err = SocketHelpers.Ioctl(_handle, extern<int>"FIONREAD", out available);
                    if (err < 0)
                        return 0;

                    return (int)Math.Min(available, Int.MaxValue);
                } else
                    build_error;
            }
        }

        EndPoint _localEndPoint, _remoteEndPoint;
        public bool IsBound { get { return _localEndPoint != null; } }
        public EndPoint LocalEndPoint { get { return _localEndPoint; } }

        private void UpdateRemoteEndPoint()
        {
            if defined(CPLUSPLUS)
            {
                _remoteEndPoint = SocketHelpers.GetRemoteEndPoint(_handle);
                if (_remoteEndPoint == null)
                    throw new SocketException(NetworkHelpers.GetError());
            }
        }

        private void UpdateLocalEndPoint()
        {
            if defined(CPLUSPLUS)
            {
                _localEndPoint = SocketHelpers.GetLocalEndPoint(_handle);
                if (_localEndPoint == null)
                    throw new SocketException(NetworkHelpers.GetError());
            }
        }

        public EndPoint RemoteEndPoint
        {
            get
            {
                if (_remoteEndPoint != null)
                    return _remoteEndPoint;

                if (!Connected)
                {
                    if (IsBound)
                        throw new SocketException("A bound socket cannot have a remote end-point");

                    return null;
                }

                return _remoteEndPoint;
            }
        }

        public void Connect(EndPoint endPoint)
        {
            var ipEndPoint = endPoint as IPEndPoint;
            if (ipEndPoint != null)
            {
                if defined(CPLUSPLUS)
                {
                    var result = SocketHelpers.Connect(_handle,
                                                       SocketHelpers.GetFamily(ipEndPoint.AddressFamily),
                                                       ipEndPoint.Address.GetAddressBytes(),
                                                       ipEndPoint.Address.ScopeId,
                                                       ipEndPoint.Port);
                    if (result < 0)
                        throw new SocketException(NetworkHelpers.GetError());

                    UpdateLocalEndPoint();
                } else
                    build_error;
            }
            else
                throw new ArgumentException("EndPoint not supported: " + endPoint);

            UpdateRemoteEndPoint();
            _connected = true;
        }

        public void Connect(string address, int port)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            Connect(endPoint);
        }

        public int Send(byte[] buffer, int offset, int length, SocketFlags flags)
        {
            if defined(CPLUSPLUS)
            {
                var ret = extern<int>(_handle, buffer, offset, length)"(@{int})send($0, (char *)$1->Ptr() + $2, $3, 0)";
                if (ret < 0)
                    throw new SocketException(NetworkHelpers.GetError());

                return ret;
            } else
                build_error;
        }

        public int Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public int Receive(byte[] buffer, int offset, int length, SocketFlags flags)
        {
            if ((offset + length) > buffer.Length)
                throw new ArgumentOutOfRangeException("Offset and length out of range");

            if defined(CPLUSPLUS)
            {
                var ret = extern<int>(_handle, buffer, offset, length)"(@{int})recv($0, (char *)$1->Ptr() + $2, $3, 0)";
                if (ret < 0)
                    throw new SocketException(NetworkHelpers.GetError());

                return ret;
            } else
                build_error;
        }

        public int Receive(byte[] buffer)
        {
            return Receive(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public void Bind(EndPoint endPoint)
        {
            var ipEndPoint = endPoint as IPEndPoint;
            if (ipEndPoint != null)
            {
                if defined(CPLUSPLUS)
                {
                    var result = SocketHelpers.Bind(_handle,
                                                    SocketHelpers.GetFamily(ipEndPoint.AddressFamily),
                                                    ipEndPoint.Address.GetAddressBytes(),
                                                    ipEndPoint.Address.ScopeId,
                                                    ipEndPoint.Port);
                    if (result < 0)
                        throw new SocketException(NetworkHelpers.GetError());

                    UpdateLocalEndPoint();
                } else
                    build_error;
            }
            else
            {
                throw new ArgumentException("EndPoint not supported: " + endPoint);
            }
        }

        public void Listen(int backlog)
        {
            if defined(CPLUSPLUS)
            {
                var result = extern<int>(_handle, backlog) "listen($0, $1)";
                if (result < 0)
                    throw new SocketException(NetworkHelpers.GetError());
            } else
                build_error;
        }

        public bool Poll(int milliseconds, SelectMode mode)
        {
            if defined(CPLUSPLUS)
            {
                int result = SocketHelpers.Poll(_handle, milliseconds, mode);
                if (result < 0)
                    throw new SocketException(NetworkHelpers.GetError());

                return result > 0;
            } else
                build_error;
        }

        public void Shutdown(SocketShutdown how)
        {
            if (!_connected)
                throw new SocketException("Not connected!");

            if defined(CPLUSPLUS)
            {
                var result = SocketHelpers.Shutdown(_handle, SocketHelpers.GetSocketShudown(how));
                if (result < 0)
                    throw new SocketException(NetworkHelpers.GetError());
            } else
                build_error;

            _connected = false;
        }

        public Socket Accept()
        {
            if defined(CPLUSPLUS)
            {
                var result = SocketHelpers.Accept(_handle);
                if (result == SocketHandle.Invalid)
                    throw new SocketException(NetworkHelpers.GetError());

                return new Socket(result, true);
            } else
                build_error;
        }

        public void Close()
        {
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if defined(CPLUSPLUS)
            {
                int result;
                if defined(MSVC)
                    result = extern<int>(_handle) "closesocket($0)";
                else
                    result = extern<int>(_handle) "close($0)";

                if (result < 0)
                    throw new SocketException(NetworkHelpers.GetError());
            } else
                build_error;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~Socket()
        {
            Dispose(false);
        }
    }

    [DotNetType("System.Net.Sockets.SocketException")]
    public class SocketException : Uno.Exception
    {
        internal SocketException(string message) : base(message)
        {
        }
    }
}
